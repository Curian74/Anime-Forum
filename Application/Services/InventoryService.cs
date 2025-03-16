using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class InventoryService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGenericRepository<User> _userRepository = unitOfWork.GetRepository<User>();
        private readonly IGenericRepository<UserInventory> _inventoryRepository = unitOfWork.GetRepository<UserInventory>();
        private readonly IGenericRepository<UserFlair> _flairRepository = unitOfWork.GetRepository<UserFlair>();
        private readonly IGenericRepository<UserFlairSelection> _flairSelectionRepository = unitOfWork.GetRepository<UserFlairSelection>();

        public async Task<UserInventory> GetUserInventoryAsync(Guid userId)
        {
            var inventory = await _inventoryRepository.GetSingleWhereAsync(i => i.UserId == userId);
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (inventory == null)
            {
                inventory = new UserInventory 
                {
                    UserId = userId,
                };
                await _inventoryRepository.AddAsync(inventory);
                await _unitOfWork.SaveChangesAsync();
            }

            await CheckUnlockedFlairsAsync(user, inventory);

            return inventory;
        }

        public async Task<UserFlair?> GetActiveFlairAsync(Guid? userId)
        {
            var selection = await _flairSelectionRepository.GetSingleWhereAsync(s => s.UserId == userId);
            return selection?.Flair;
        }

        public async Task<bool> SetActiveFlairAsync(Guid userId, Guid flairId)
        {
            var userInventory = await GetUserInventoryAsync(userId);

            if (!userInventory.Flairs.Any(f => f.Id == flairId))
                return false; // User does not own requested flair

            var existingSelection = await _flairSelectionRepository.GetSingleWhereAsync(s => s.UserId == userId);
            if (existingSelection != null)
            {
                await _flairSelectionRepository.DeleteAsync(existingSelection.Id);
            }

            var newSelection = new UserFlairSelection
            {
                UserId = userId,
                FlairId = flairId
            };

            await _flairSelectionRepository.AddAsync(newSelection);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<UserFlair?> GetFlairByIdAsync(Guid flairId)
        {
            return await _flairRepository.GetByIdAsync(flairId);
        }

        public async Task<bool> AddFlairToInventoryAsync(Guid userId, Guid flairId)
        {
            var inventory = await GetUserInventoryAsync(userId);
            var flair = await GetFlairByIdAsync(flairId);

            if (flair == null || inventory.Flairs.Any(f => f.Id == flairId))
                return false; // Flair does not exist or is already in inventory

            inventory.Flairs.Add(flair);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task UpdateFlairsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var inventory = await GetUserInventoryAsync(userId);

            await CheckUnlockedFlairsAsync(user, inventory);
        }

        private async Task CheckUnlockedFlairsAsync(User user, UserInventory inventory)
        {
            var userFlairs = inventory.Flairs.ToList();
            var (allFlairs, _) = await _flairRepository.GetAllAsync();

            // Remove flairs the user no longer qualifies for
            foreach (var flair in userFlairs)
            {
                if (flair.PointsRequired > user.Points)
                {
                    inventory.Flairs.Remove(flair);

                    // If the currently selected flair is now locked, remove selection
                    if (user.UserFlairSelection?.FlairId == flair.Id)
                    {
                        await _flairSelectionRepository.DeleteAsync(user.UserFlairSelection.Id);
                        user.UserFlairSelection = null;
                    }
                }
            }

            // Unlock new flairs based on the updated points
            foreach (var flair in allFlairs)
            {
                if (flair.PointsRequired <= user.Points && !inventory.Flairs.Any(f => f.Id == flair.Id))
                {
                    inventory.Flairs.Add(flair);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

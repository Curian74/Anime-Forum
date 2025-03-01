using Application.Common.MessageOperations;
using Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Session
{
	public class OTPValidation
	{
		public Dictionary<string, string> ValidateOTP(string savedOTP, string OTP, string expiryTimeStr)
		{
			Dictionary<string, string> errors = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(expiryTimeStr))
			{
				errors.Add("OTP", MessageConstants.MEO002);
				return errors;
			}
			if (!DateTime.TryParse(expiryTimeStr, out DateTime expiryTime))
			{
				errors.Add("OTP", MessageConstants.MEO001);
				return errors;
			}		
			if (DateTime.UtcNow > expiryTime)
			{
				errors.Add("OTP", MessageConstants.MEN007);
				return errors;
			}
			if (!savedOTP.Equals(OTP))
			{
				errors.Add("OTP", MessageConstants.MEN006);
				return errors;
			}
			return errors;
		}
	}
}

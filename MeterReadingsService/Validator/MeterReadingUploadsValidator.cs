using System;
using System.Collections.Generic;
using MeterReadingsService.Repositories;

namespace MeterReadingsService.Validator
{
    public class MeterReadingUploadsValidator : IMeterReadingUploadsValidator
    {
        private IAccountsRepository _AccountsRepository;

        public MeterReadingUploadsValidator(IAccountsRepository accountsRepository)
        {
            _AccountsRepository = accountsRepository;

        }
        public bool IsValid(dynamic csvRow)
        {
            var properties =  (IDictionary<string, object>) csvRow;

            var isValid = ValidateNumberOfProperties(properties) &&
                          ValidateCorrectProperties(properties) &&
                          ValidateAccountId(properties) &&
                          ValidateMeterReadingDateTime(properties) &&
                          ValidateMeterReadValue(properties);

            return isValid;
        }


        private bool ValidateNumberOfProperties(IDictionary<string, object> properties)
        {
            return properties.Count == 3;
        }

        private bool ValidateCorrectProperties(IDictionary<string, object> properties)
        {
            return properties.ContainsKey("AccountId") &&
                   properties.ContainsKey("MeterReadingDateTime") &&
                   properties.ContainsKey("MeterReadValue");
        }

        private bool ValidateAccountId(IDictionary<string, object> properties)
        {
            var accountId = properties["AccountId"].ToString();

            int accountIdInt;

            return accountId != null &&
                   int.TryParse(accountId, out accountIdInt) &&
                   _AccountsRepository.Exists(accountIdInt);
        }

        private bool ValidateMeterReadingDateTime(IDictionary<string, object> properties)
        {
            var meterReadValue = properties["MeterReadingDateTime"].ToString();

            return meterReadValue != null &&
                   DateTime.TryParse(meterReadValue, out _);
        }

        private bool ValidateMeterReadValue(IDictionary<string, object> properties)
        {
            var meterRead = properties["MeterReadValue"].ToString();
            var numberOfCharacters = meterRead?.Length;
            var isInt = int.TryParse(meterRead, out var meterReadValue);


            return meterRead != null &&
                   numberOfCharacters > 0 &&
                   numberOfCharacters <= 5 &&
                   isInt &&
                   meterReadValue > 0;
        }
    }
}

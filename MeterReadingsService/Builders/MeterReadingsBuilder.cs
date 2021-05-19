using System;
using System.Collections.Generic;
using System.Linq;
using MeterReadingsService.Models;
using MeterReadingsService.Validator;

namespace MeterReadingsService.Builders
{
    public class MeterReadingsBuilder : IMeterReadingsBuilder
    {
        private IMeterReadingUploadsValidator _validator;

        public MeterReadingsBuilder(IMeterReadingUploadsValidator validator)
        {
            _validator = validator;
        }

        public List<MeterReading> Build(List<dynamic> csvRows)
        {
            if (csvRows == null)
            {
                throw new ArgumentNullException(nameof(csvRows));
            }

            if (csvRows.Count == 0)
            {
                throw new ArgumentException($"{nameof(csvRows)} should contain at least 1 row");
            }

            var meterReadings = new List<MeterReading>();
            
            foreach (var row in csvRows)
            {

                var isValid = _validator.IsValid(row);

                if (isValid)
                {
                    var meterReading = new MeterReading
                    {
                        AccountId = (int)row.AccountId,
                        MeterReadingDateTime = (DateTime)row.MeterReadingDateTime,
                        MeterReadValue = (int)row.MeterReadValue
                    };

                    meterReadings.Add(meterReading);
                }
               
            }

            var uniqueMeterReadings = RemoveDuplicateReadings(meterReadings);

            return uniqueMeterReadings;
        }

        private List<MeterReading> RemoveDuplicateReadings(List<MeterReading> meterReadings)
        {
            var uniqueMeterReadings = new List<MeterReading>();
            
            var readingsByAccount = meterReadings.GroupBy(x => x.AccountId);

            foreach (var readings in readingsByAccount)
            {
                var readingsByValue = readings.GroupBy(x => x.MeterReadValue);

                foreach (var readingValue in readingsByValue)
                {
                    uniqueMeterReadings.Add(readingValue.OrderBy(x => x.MeterReadingDateTime).Last());
                }
            }

            return uniqueMeterReadings;
        }
    }
}

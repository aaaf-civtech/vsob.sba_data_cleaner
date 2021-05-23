using ChoETL;
using Microsoft.VisualBasic.CompilerServices;

namespace vsob.sba_data_cleaner
{
    [ChoCSVFileHeader]
    public class SbaVosbRecord
    {
        [ChoCSVRecordField(2, FieldName = "Name of Firm")]
        public string CompanyName { get; set; } 
    }
}
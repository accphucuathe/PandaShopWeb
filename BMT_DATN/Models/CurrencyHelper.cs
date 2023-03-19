using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BMT_DATN.Models
{
    public class CurrencyHelper
    {
        public String FormatToCurrency (int moneyNumber)
        {
            string moneyValue = String.Format(new CultureInfo("vi-VN"), "{0:C0}", moneyNumber);
            return moneyValue;
        }
        
        public String FormatWithComma (int moneyNumber)
        {
            string moneyValue = String.Format("{0:N0}", moneyNumber);
            return moneyValue;
        }

        public int FormatToNumber (String moneyText)
        {
            int moneyvalue2 = int.Parse(moneyText, NumberStyles.Currency);
            return moneyvalue2;
        }
    }
}
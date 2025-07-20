using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListUIBlazor.Utilities;

public static class RandomExtensions
{
    public static int DaysUnilChristmas(this DateTime dateTime)
    {
        DateTime christmas = new(dateTime.Year, 12, 25);

        return (christmas - dateTime).Days;
    }
}

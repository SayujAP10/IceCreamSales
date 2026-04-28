using System;
using System.Collections.Generic;


class Program
{

    record SaleRow(DateTime Date, string SKU, decimal UnitPrice, int Quantity, decimal TotalPrice);

    static string rawData = @"Date,SKU,Unit Price,Quantity,Total Price
                              2019-01-01,Death by Chocolate,180,5,900
                              2019-01-01,Cake Fudge,150,1,150
                              2019-01-01,Cake Fudge,150,1,150
                              2019-01-01,Cake Fudge,150,3,450
                              2019-01-01,Death by Chocolate,180,1,180
                              2019-01-01,Vanilla Double Scoop,80,3,240
                              2019-01-01,Butterscotch Single Scoop,60,5,300
                              2019-01-01,Vanilla Single Scoop,50,5,250
                              2019-01-01,Cake Fudge,150,5,750
                              2019-01-01,Hot Chocolate Fudge,120,3,360
                              2019-01-01,Butterscotch Single Scoop,60,5,300
                              2019-01-01,Chocolate Europa Double Scoop,100,1,100
                              2019-01-01,Hot Chocolate Fudge,120,2,240
                              2019-01-01,Caramel Crunch Single Scoop,70,4,280
                              2019-01-01,Hot Chocolate Fudge,120,2,240
                              2019-01-01,Hot Chocolate Fudge,120,4,480
                              2019-01-01,Hot Chocolate Fudge,120,2,240
                              2019-01-01,Cafe Caramel,160,5,800
                              2019-01-01,Vanilla Double Scoop,80,4,320
                              2019-01-01,Butterscotch Single Scoop,60,3,180
                              2019-02-01,Butterscotch Single Scoop,60,3,180
                              2019-02-01,Vanilla Single Scoop,50,2,100
                              2019-02-01,Butterscotch Single Scoop,60,3,180
                              2019-02-01,Vanilla Double Scoop,80,1,80
                              2019-02-01,Death by Chocolate,180,2,360
                              2019-02-01,Cafe Caramel,160,2,320
                              2019-02-01,Pista Single Scoop,60,3,180
                              2019-02-01,Hot Chocolate Fudge,120,2,240
                              2019-02-01,Vanilla Single Scoop,50,3,150
                              2019-02-01,Vanilla Single Scoop,50,5,250
                              2019-02-01,Cake Fudge,150,1,150
                              2019-02-01,Vanilla Single Scoop,50,4,200
                              2019-02-01,Vanilla Double Scoop,80,3,240
                              2019-02-01,Cake Fudge,150,1,150
                              2019-02-01,Vanilla Double Scoop,80,5,400
                              2019-02-01,Hot Chocolate Fudge,120,5,600
                              2019-02-01,Vanilla Double Scoop,80,2,160
                              2019-02-01,Vanilla Double Scoop,80,3,240
                              2019-02-01,Hot Chocolate Fudge,120,5,600
                              2019-02-01,Cake Fudge,150,5,750
                              2019-03-01,Vanilla Single Scoop,50,5,250
                              2019-03-01,Cake Fudge,150,5,750
                              2019-03-01,Pista Single Scoop,60,1,60
                              2019-03-01,Butterscotch Single Scoop,60,2,120
                              2019-03-01,Vanilla Double Scoop,80,1,80
                              2019-03-01,Cafe Caramel,160,1,160
                              2019-03-01,Cake Fudge,150,5,750
                              2019-03-01,Trilogy,160,5,800
                              2019-03-01,Butterscotch Single Scoop,60,3,180
                              2019-03-01,Death by Chocolate,180,2, 360
                              2019-03-01,Butterscotch Single Scoop,60,1,60
                              2019-03-01,Hot Chocolate Fudge,120,3,360
                              2019-03-01,Cake Fudge,150,2,300
                              2019-03-01,Cake Fudge,150,2,300
                              2019-03-01,Vanilla Single Scoop,50,4,100
                              2019-03-01,Cafe Caramel,160,0,160
                              2019-03-01,Cake Fudge,150,5,750
                              2019-03-01,Cafe Caramel,160,5,800
                              2019-03-01,Almond Fudge,150,1,150
                              2019-03-01,Cake Fudge,150,1,150";

static void Main(string[] args)
{
        var (rows, errors) = ParseData();

        TotalSales(rows);
        MonthWiseSales(rows);
        MostPopularItem(rows);
        MostRevenueItem(rows);
        GrowthByMonth(rows);

        Console.WriteLine("\n****** DATA INCONSISTENCIES ******");
        if (errors.Count == 0)
        {
            Console.WriteLine("  No errors found.");
        }
        else
        {
            foreach (var e in errors)
                Console.WriteLine("  " + e);
        }
        Console.ReadKey();
    }

    static (List<SaleRow> rows, List<string> errors) ParseData()
    {
        var rows = new List<SaleRow>();
        var errors = new List<string>();
        var lines = rawData.Trim().Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line == "") continue;

            var parts = line.Split(',');
            var error = ValidateRow(parts, i);

            if (error != null) 
            { 
                errors.Add(error); 
                continue;
            }

            rows.Add(new SaleRow(
                DateTime.Parse(parts[0].Trim()),
                parts[1].Trim(),
                decimal.Parse(parts[2].Trim()),
                int.Parse(parts[3].Trim()),
                decimal.Parse(parts[4].Trim())
            ));
        }

        return (rows, errors);
    }

    static string ValidateRow(string[] parts, int rowNum)
    {
        if (parts.Length < 5)
            return $"Row {rowNum}: Not enough columns";

        if (!DateTime.TryParse(parts[0].Trim(), out _))
            return $"Row {rowNum}: Invalid date '{parts[0]}'";

        if (!decimal.TryParse(parts[2].Trim(), out decimal unitPrice) || unitPrice < 0)
            return $"Row {rowNum}: Invalid unit price '{parts[2]}'";

        if (!int.TryParse(parts[3].Trim(), out int qty) || qty < 1)
            return $"Row {rowNum}: Quantity should not be zero'{parts[3]}'";

        if (!decimal.TryParse(parts[4].Trim(), out decimal total) || total < 0)
            return $"Row {rowNum}: Total should not be zero '{parts[4]}'";

        if (unitPrice * qty != total)
            return $"Row {rowNum}: Mismatch in unit price : {unitPrice} x {qty} != {total}";

        return null;
    }

    static void TotalSales(List<SaleRow> rows)
    {
        decimal total = 0;
        foreach (var row in rows) 
            total += row.TotalPrice;

        Console.WriteLine("****** Total Sales ******");
        Console.WriteLine($"  Grand Total : {total}");
    }

    static void MonthWiseSales(List<SaleRow> rows)
    {
        var monthTotals = new Dictionary<string, decimal>();

        foreach (var row in rows)
        {
            string month = row.Date.ToString("yyyy-MM");
            if (!monthTotals.ContainsKey(month)) monthTotals[month] = 0;
            monthTotals[month] += row.TotalPrice;
        }

        Console.WriteLine("\n****** Month-wise Sales ******");
        foreach (var entry in monthTotals)
            Console.WriteLine($"  {entry.Key}: {entry.Value}");
    }

    static void MostPopularItem(List<SaleRow> rows)
    {
        var data = new Dictionary<string, Dictionary<string, List<int>>>();

        foreach (var row in rows)
        {
            string month = row.Date.ToString("yyyy-MM");
            if (!data.ContainsKey(month))
                data[month] = new Dictionary<string, List<int>>();
            if (!data[month].ContainsKey(row.SKU))
                data[month][row.SKU] = new List<int>();
            data[month][row.SKU].Add(row.Quantity);
        }

        Console.WriteLine("\n****** Most Popular Item per Month ******");

        foreach (var month in data.Keys)
        {
            string topSKU = null;
            int topQty = 0;

            foreach (var sku in data[month].Keys)
            {
                int sum = 0;
                foreach (var q in data[month][sku]) sum += q;
                if (sum > topQty) 
                { 
                    topQty = sum; 
                    topSKU = sku; 
                }
            }

            var orders = data[month][topSKU];
            int min = int.MaxValue, max = int.MinValue, total = 0;

            foreach (var q in orders)
            {
                if (q < min) min = q;
                if (q > max) max = q;
                total += q;
            }

            double avg = (double)total / orders.Count;

            Console.WriteLine($"  {month}: {topSKU}");
            Console.WriteLine($"    Total Qty: {topQty} | Min: {min} | Max: {max} | Avg: {avg:F1}");
        }
    }

    static void MostRevenueItem(List<SaleRow> rows)
    {
        var data = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var row in rows)
        {
            string month = row.Date.ToString("yyyy-MM");
            if (!data.ContainsKey(month))
                data[month] = new Dictionary<string, decimal>();
            if (!data[month].ContainsKey(row.SKU))
                data[month][row.SKU] = 0;
            data[month][row.SKU] += row.TotalPrice;
        }

        Console.WriteLine("\n****** Most Revenue Item per Month ******");

        foreach (var month in data.Keys)
        {
            string topSKU = null;
            decimal topRev = 0;

            foreach (var sku in data[month].Keys)
                if (data[month][sku] > topRev)
                { 
                    topRev = data[month][sku]; 
                    topSKU = sku; 
                }

            Console.WriteLine($"  {month}: {topSKU} -> {topRev}");
        }
    }
    static void GrowthByMonth(List<SaleRow> rows)
    {
        var data = new Dictionary<string, Dictionary<string, decimal>>();

        foreach (var row in rows)
        {
            string month = row.Date.ToString("yyyy-MM");
            if (!data.ContainsKey(month))
                data[month] = new Dictionary<string, decimal>();
            if (!data[month].ContainsKey(row.SKU))
                data[month][row.SKU] = 0;
            data[month][row.SKU] += row.TotalPrice;
        }

        var months = new List<string>(data.Keys);
        months.Sort();

        Console.WriteLine("\n****** Month-to-Month Growth ******");

        for (int i = 1; i < months.Count; i++)
        {
            string prev = months[i - 1];
            string curr = months[i];

            var allSKUs = new HashSet<string>();
            foreach (var s in data[prev].Keys) allSKUs.Add(s);
            foreach (var s in data[curr].Keys) allSKUs.Add(s);

            foreach (var sku in allSKUs)
            {
                data[prev].TryGetValue(sku, out decimal prevRev);
                data[curr].TryGetValue(sku, out decimal currRev);

                if (prevRev == 0) continue;

                double growth = (double)((currRev - prevRev) / prevRev) * 100;
                Console.WriteLine($"  {sku} | {prev} -> {curr}: {growth:+0.00;-0.00}%");
            }
        }
    }
}

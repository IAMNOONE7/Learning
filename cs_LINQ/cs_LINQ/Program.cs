using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_LINQ
{
    /*
    =============================================================
    C# LINQ 
    =============================================================

    This console project demonstrates how to use LINQ (Language Integrated Query)
    in C# to query, filter, sort, group, and transform data in memory.
    Each LINQ concept is implemented as a separate function with comments,
    and all are called from the Main() method.

    LINQ allows querying data from:
     - In-memory collections (Lists, Arrays)
     - Databases (via Entity Framework - LINQ to Entities)
     - XML documents (LINQ to XML)
     - JSON and other data sources

    It unifies data manipulation in a consistent, SQL-like syntax.

    -------------------------------------------------------------
    STRUCTURE:
    -------------------------------------------------------------
    Main()
     LinqWhere()                - Filtering elements
     LinqSelect()               - Projecting / transforming data
     LinqOrderBy()              - Sorting results
     LinqGroupBy()              - Grouping by key
     LinqJoin()                 - Joining two sequences
     LinqAggregate()            - Summarizing values (Sum, Count, etc.)
     LinqQuantifiers()          - Checking conditions (Any, All, Contains)
     LinqProjectionAnonymous()  - Using anonymous types in results
     LinqSelectMany()           - Flattening nested collections
     LinqDeferredExecution()    - Demonstrating lazy evaluation
     LinqComplexQuery()         - Combining multiple LINQ operations
     

    -------------------------------------------------------------
    MAIN LINQ CONCEPTS:
    -------------------------------------------------------------
    1️)  Filtering:
         - Where - Selects elements that match a condition.

    2️)  Projection (Transformation):
         - Select(selector) - Transforms elements into a new form or type.
         - SelectMany(selector) - Flattens nested sequences into a single sequence.

    3️)  Sorting:
         - OrderBy(keySelector)
         - OrderByDescending(keySelector)
         - ThenBy / ThenByDescending - Secondary sorting.

    4️)  Grouping:
         - GroupBy(keySelector) - Groups elements by a key.
         - Each group can be enumerated separately.

    5️)  Joining:
         - Join(inner, outerKey, innerKey, resultSelector)
         - GroupJoin(inner, outerKey, innerKey, resultSelector)
         - Useful to merge data from two collections.

    6️)  Aggregation:
         - Count(), Sum(), Average(), Min(), Max()
         - Aggregate(seed, func) - Custom accumulation logic.

    7️)  Quantifiers:
         - Any - True if any element matches.
         - All - True if all elements match.
         - Contains(value) - True if sequence contains the given value.

    8️)  Set Operations:
         - Distinct() - Remove duplicates.
         - Union(second) - Combine two sequences, remove duplicates.
         - Intersect(second) - Common elements between sequences.
         - Except(second) - Elements in first, not in second.

    9️)  Element Operators:
         - First(), FirstOrDefault()
         - Single(), SingleOrDefault()
         - ElementAt(index)

    10)  Conversion:
         - ToList(), ToArray(), ToDictionary()
         - These execute the query immediately.

    -------------------------------------------------------------
    EXECUTION BEHAVIOR:
    -------------------------------------------------------------
    - Deferred Execution - Queries run only when enumerated
      (e.g., in a foreach loop or when ToList() is called).
    - Immediate Execution - Queries that return a scalar value
      (e.g., Count(), Sum(), Average()) are executed right away.

    -------------------------------------------------------------
    EXAMPLE:
    -------------------------------------------------------------
    var result = products
        .Where(p => p.Price > 1.0)
        .OrderBy(p => p.Category)
        .ThenByDescending(p => p.Price)
        .GroupBy(p => p.Category)
        .Select(g => new
        {
            Category = g.Key,
            Average = g.Average(p => p.Price),
            Count = g.Count()
        });

    foreach (var item in result)
        Console.WriteLine($"{item.Category}: {item.Count} items, Avg={item.Average}");

    -------------------------------------------------------------
    NOTES:
    -------------------------------------------------------------
    - Every LINQ operation returns an IEnumerable<T> (except aggregates).
    - LINQ works natively with generic collections.
    - You can mix query syntax (SQL-like) and method syntax (lambda-based).
    - Complex queries remain readable when broken into multiple steps.
    - For database queries (Entity Framework), LINQ translates into SQL.

    -------------------------------------------------------------
    GOAL OF THIS PROJECT:
    -------------------------------------------------------------
    - Build intuition for chaining LINQ operations.
    - Understand deferred execution.
    - Learn how to express complex data transformations in C#.
    - Gain practical skills that translate directly to real applications:
      WPF, Web APIs, data processing, and reporting.

    =============================================================
    */

    // Simple model for object-based LINQ examples
    public class Product
    {
        public int Id { get; set; }          // used for joins
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
    }

    public class CategoryInfo
    {
        public string Category { get; set; }
        public string Description { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // prepare sample data once and pass it to methods
            var numbers = GetNumbers();
            var products = GetProducts();
            var categories = GetCategories();

            // Call all demo methods here
            LinqWhere(numbers, products);
            LinqSelect(products);
            LinqOrderBy(products);
            LinqGroupBy(products);
            LinqJoin(products, categories);
            LinqAggregate(numbers);
            LinqQuantifiers(products);
            LinqProjectionAnonymous(products);
            LinqSelectMany();
            LinqDeferredExecution(numbers);
            LinqComplexQuery(products);

            Console.WriteLine("\n--- Done ---");
            Console.ReadKey();
        }
        // =========================
        //  SAMPLE DATA
        // =========================
        static List<int> GetNumbers() => new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        static List<Product> GetProducts() => new()
        {
            new() { Id = 1, Name = "Apple",  Category = "Fruit",     Price = 1.2 },
            new() { Id = 2, Name = "Banana", Category = "Fruit",     Price = 0.8 },
            new() { Id = 3, Name = "Carrot", Category = "Vegetable", Price = 0.6 },
            new() { Id = 4, Name = "Milk",   Category = "Dairy",     Price = 1.5 },
            new() { Id = 5, Name = "Cheese", Category = "Dairy",     Price = 2.8 },
            new() { Id = 6, Name = "Orange", Category = "Fruit",     Price = 1.1 }
        };

        static List<CategoryInfo> GetCategories() => new()
        {
            new() { Category = "Fruit", Description = "Sweet and fresh" },
            new() { Category = "Vegetable", Description = "Healthy greens" },
            new() { Category = "Dairy", Description = "Milk products" }
        };

        // =========================
        //  LINQ DEMO METHODS
        // =========================
        
        // WHERE - filtering items       
        static void LinqWhere(List<int> numbers, List<Product> products)
        {
            Console.WriteLine("=== LINQ WHERE ===");

            // Example 1: filter numbers > 5
            var biggerThanFive = numbers.Where(n => n > 5);
            Console.WriteLine("Numbers > 5: " + string.Join(", ", biggerThanFive));

            // Example 2: filter products by category and price
            var cheapFruits = products
                .Where(p => p.Category == "Fruit" && p.Price < 1.2);

            Console.WriteLine("Cheap fruits (<1.2):");
            foreach (var p in cheapFruits)
                Console.WriteLine($" - {p.Name} ({p.Price})");
        }
       
        // SELECT - projecting (transforming) data
       
        static void LinqSelect(List<Product> products)
        {
            Console.WriteLine("\n=== LINQ SELECT ===");

            // we only want product names (not the whole object)
            var names = products.Select(p => p.Name);

            Console.WriteLine("Product names:");
            foreach (var name in names)
                Console.WriteLine($" - {name}");

            // project into anonymous object with calculated field
            var nameWithVat = products.Select(p => new
            {
                p.Name,
                PriceWithVat = p.Price * 1.21   // e.g. 21% VAT
            });

            Console.WriteLine("Product with VAT:");
            foreach (var item in nameWithVat)
                Console.WriteLine($" - {item.Name}: {item.PriceWithVat:F2}");
        }
        
        // ORDERBY - sorting        
        static void LinqOrderBy(List<Product> products)
        {
            Console.WriteLine("\n=== LINQ ORDERBY ===");

            // sort by price ascending
            var byPrice = products.OrderBy(p => p.Price);
            Console.WriteLine("Sorted by price ASC:");
            foreach (var p in byPrice)
                Console.WriteLine($" - {p.Name} ({p.Price})");

            // sort by category, then by name
            var byCategoryThenName = products
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name);

            Console.WriteLine("Sorted by category, then name:");
            foreach (var p in byCategoryThenName)
                Console.WriteLine($" - {p.Category}: {p.Name}");
        }
        
        // GROUPBY - group items under a key       
        static void LinqGroupBy(List<Product> products)
        {
            Console.WriteLine("\n=== LINQ GROUPBY ===");

            var grouped = products.GroupBy(p => p.Category);

            foreach (var group in grouped)
            {
                Console.WriteLine($"Category: {group.Key}"); // group.Key is the category
                foreach (var p in group)
                    Console.WriteLine($" - {p.Name} ({p.Price})");
            }
        }
       
        // JOIN - combine data from two sequences        
        static void LinqJoin(List<Product> products, List<CategoryInfo> categories)
        {
            Console.WriteLine("\n=== LINQ JOIN ===");

            // join products with category info based on the Category name
            var joined = products.Join(
                categories,
                p => p.Category,           // outer key selector (from products)
                c => c.Category,           // inner key selector (from categories)
                (p, c) => new              // result selector - what we want back
                {
                    p.Name,
                    p.Category,
                    p.Price,
                    c.Description
                });

            foreach (var item in joined)
                Console.WriteLine($" - {item.Name} [{item.Category}] - {item.Description} ({item.Price})");
        }
       
        // AGGREGATE (Count, Sum, Average, Min, Max)        
        static void LinqAggregate(List<int> numbers)
        {
            Console.WriteLine("\n=== LINQ AGGREGATE ===");

            Console.WriteLine($"Count: {numbers.Count()}");
            Console.WriteLine($"Sum: {numbers.Sum()}");
            Console.WriteLine($"Average: {numbers.Average()}");
            Console.WriteLine($"Min: {numbers.Min()}");
            Console.WriteLine($"Max: {numbers.Max()}");

            // custom Aggregate - factorial-like demo
            int product = numbers.Aggregate(1, (acc, n) => acc * n); // careful: 1*2*3*...*9
            Console.WriteLine($"Aggregate product (1..9): {product}");
        }
      
        // ANY / ALL / CONTAINS
        
        static void LinqQuantifiers(List<Product> products)
        {
            Console.WriteLine("\n=== LINQ QUANTIFIERS ===");

            // Is there any dairy product more expensive than 2?
            bool anyExpensiveDairy = products.Any(p => p.Category == "Dairy" && p.Price > 2);
            Console.WriteLine($"Any expensive dairy? {anyExpensiveDairy}");

            // Are all products over 0?
            bool allPositive = products.All(p => p.Price > 0);
            Console.WriteLine($"All products have positive price? {allPositive}");

            // Contains works on simple collections
            var nums = new[] { 1, 2, 3, 4 };
            Console.WriteLine($"Numbers contain 3? {nums.Contains(3)}");
        }
        
        // SELECT with anonymous types       
        static void LinqProjectionAnonymous(List<Product> products)
        {
            Console.WriteLine("\n=== LINQ ANONYMOUS PROJECTION ===");

            var projected = products.Select(p => new
            {
                Label = $"{p.Name} ({p.Category})",
                IsExpensive = p.Price > 1.5                
            });

            foreach (var item in projected)
                Console.WriteLine($" - {item.Label}, Expensive: {item.IsExpensive}");
        }
        
        // SELECTMANY - flattening lists of lists
        // (simple demo just to show the operator)       
        static void LinqSelectMany()
        {
            Console.WriteLine("\n=== LINQ SELECTMANY ===");

            var listOfLists = new List<List<int>>
            {
                new() { 1, 2, 3 },
                new() { 4, 5 },
                new() { 6, 7, 8, 9 }
            };

            // SelectMany will "flatten" the inner lists into one sequence
            var flat = listOfLists.SelectMany(list => list);

            Console.WriteLine("Flattened: " + string.Join(", ", flat));
        }
       
        // Deferred execution demo        
        static void LinqDeferredExecution(List<int> numbers)
        {
            Console.WriteLine("\n=== LINQ DEFERRED EXECUTION ===");

            // build query
            var query = numbers.Where(n => n > 3);

            // change source AFTER we defined query
            numbers.Add(100);

            // query is executed here (foreach)
            foreach (var n in query)
                Console.WriteLine(n);

            // if we wanted to "freeze" result, we would do: var list = query.ToList();
        }

       
        // COMPLEX LINQ QUERY DEMO
        // Demonstrates how multiple LINQ operations can be chained together to build
        // a powerful data transformation pipeline.

        // Steps performed:
        // 1. Filter - Select only fruits and dairy items that cost more than 1.0
        // 2. Order - Sort first by Category, then by descending Price
        // 3. Group - Group by Category
        // 4. Project - Create a new anonymous type with summary data (average price, count)
        // 5. SelectMany - Flatten grouped results for display
        // </summary>


        static void LinqComplexQuery(List<Product> products)
        {
            Console.WriteLine("\n=== COMPLEX LINQ QUERY ===");

            // Step 1: Filter data
            var filtered = products
                .Where(p => (p.Category == "Fruit" || p.Category == "Dairy") && p.Price > 1.0);

            // Step 2: Order data
            var ordered = filtered
                .OrderBy(p => p.Category)
                .ThenByDescending(p => p.Price);

            // Step 3: Group by category
            var grouped = ordered
                .GroupBy(p => p.Category)
                .Select(group => new
                {
                    Category = group.Key,
                    AveragePrice = group.Average(p => p.Price),
                    ProductCount = group.Count(),
                    Products = group.Select(p => new
                    {
                        p.Name,
                        p.Price
                    })
                });

            // Step 4: Print group summary
            foreach (var group in grouped)
            {
                Console.WriteLine($"\nCategory: {group.Category}");
                Console.WriteLine($"  Avg Price: {group.AveragePrice:F2}, Count: {group.ProductCount}");

                // Step 5: Print detailed products
                foreach (var p in group.Products)
                    Console.WriteLine($"   - {p.Name}: {p.Price:F2}");
            }

            // Step 6: Combine everything in one chained expression (compact form)
            Console.WriteLine("\n--- Compact One-Liner ---");
            var compact =
                products
                .Where(p => (p.Category == "Fruit" || p.Category == "Dairy") && p.Price > 1.0)
                .OrderBy(p => p.Category)
                .ThenByDescending(p => p.Price)
                .GroupBy(p => p.Category)
                .Select(g => new
                {
                    g.Key,
                    Count = g.Count(),
                    Avg = g.Average(p => p.Price)
                });

            foreach (var item in compact)
                Console.WriteLine($"[{item.Key}] Count={item.Count}, Avg={item.Avg:F2}");
        }
    }
}

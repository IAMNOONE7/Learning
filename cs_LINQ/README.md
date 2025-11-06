# C# LINQ Demo

This project demonstrates how to use **LINQ (Language Integrated Query)** in C#  
to query, filter, sort, group, and transform data in memory.

---

## Overview

LINQ allows you to write SQL-like queries directly in C#.  
It can be used on **lists, arrays, XML, JSON**, and even **databases** (via Entity Framework).

This console app contains multiple examples, each in its own function, showing
how LINQ can simplify data manipulation with clear, readable syntax.

---

## Features Demonstrated

| Method | Description |
|--------|--------------|
| `LinqWhere()` | Filters data using conditions |
| `LinqSelect()` | Projects data into a new form |
| `LinqOrderBy()` | Sorts by one or multiple fields |
| `LinqGroupBy()` | Groups elements by a key |
| `LinqJoin()` | Combines two collections by a key |
| `LinqAggregate()` | Calculates totals, averages, etc. |
| `LinqQuantifiers()` | Uses `Any()`, `All()`, `Contains()` |
| `LinqProjectionAnonymous()` | Creates anonymous result objects |
| `LinqSelectMany()` | Flattens nested collections |
| `LinqDeferredExecution()` | Shows deferred vs. immediate execution |
| `LinqComplexQuery()` | Combines multiple LINQ operations into one pipeline |

---

## Concepts Covered

- **Deferred Execution**  
  Queries run only when enumerated (`foreach`, `ToList()`, etc.)

- **Anonymous Types**  
  Useful for temporary, shape-only data.

- **Chaining Operators**  
  Multiple LINQ methods can be combined to express full pipelines.

---



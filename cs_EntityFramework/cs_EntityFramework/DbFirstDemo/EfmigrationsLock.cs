using System;
using System.Collections.Generic;

namespace cs_EntityFramework.DbFirstDemo;

public partial class EfmigrationsLock
{
    public int Id { get; set; }

    public string Timestamp { get; set; } = null!;
}

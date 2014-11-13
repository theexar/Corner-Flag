﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CornerFlag.Data.Models.Entities
{
    public class Stadium : NamedDatabaseEntry
    {
        public virtual Country Country { get; set; }

        public int Capacity { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XMLSoccerCOM;

namespace CornerFlag.Web.Models
{
    public class TeamPlayersViewModel
    {
        public string TeamName { get; set; }

        public IEnumerable<Player> Players { get; set; }
    }
}
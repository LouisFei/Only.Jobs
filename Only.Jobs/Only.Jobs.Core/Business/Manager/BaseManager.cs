﻿using SqlSugar;

namespace Only.Jobs.Core.Business.Manager
{
    public class BaseManager
    {
        public SqlSugarClient db
        {
            get
            {
                return new DbManager().Db;
            }
        }
    }
}

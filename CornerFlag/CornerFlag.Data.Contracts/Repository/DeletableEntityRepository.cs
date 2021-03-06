﻿namespace CornerFlag.Data.Contracts.Repository
{
    using System;
    using System.Linq;
    using System.Data.Entity;

    using CornerFlag.Data.Contracts.Models;

    public class DeletableEntityRepository<T> : GenericRepository<T>, IDeletableEntityRepository<T>
        where T : class, IDeletableEntity
    {
        public DeletableEntityRepository(DbContext context)
            : base(context)
        {
        }

        public override IQueryable<T> All()
        {
            return base.All().Where(x => !x.IsDeleted);
        }

        public IQueryable<T> AllWithDeleted()
        {
            return base.All();
        }

        public override void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.Now;
            base.ChangeEntityState(entity, EntityState.Modified);
        }

        public void HardDelete(T entity)
        {
            base.Delete(entity);
        }
    }
}

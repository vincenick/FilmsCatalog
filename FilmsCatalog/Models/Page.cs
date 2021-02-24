using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FilmsCatalog.Models
{
    public sealed class Page<T>: List<T>
    {
        public static async Task<Page<T>> CreateAsync(
            IQueryable<T> source, 
            uint pageIndex, 
            uint pageSize,
            CancellationToken token = default
        )
        {
            var count = await source.CountAsync(token);
            var itemsToSkip = (pageIndex - 1) * pageSize;
            var items = await source
                .Skip((int)itemsToSkip)
                .Take((int)pageSize)
                .ToListAsync(token);

            return new Page<T>(items, (uint)count, pageIndex, pageSize);
        }

        public uint PageIndex { get; private set; }
        public uint TotalPages { get; private set; }
        public bool HasPrevious { get => PageIndex > 1; }
        public bool HasNext { get => PageIndex < TotalPages; }

        public Page(List<T> items, uint count, uint pageIndex, uint pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (uint)Math.Ceiling(count / (double) pageSize);

            this.AddRange(items);
        }
    }
}
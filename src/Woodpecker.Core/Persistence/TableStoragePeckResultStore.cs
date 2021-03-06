﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.Persistence
{
    public class TableStoragePeckResultStore : IPeckResultStore
    {
        public async Task StoreAsync(PeckSource source, IEnumerable<ITableEntity> results)
        {
            var account = CloudStorageAccount.Parse(source.DestinationConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference(source.DestinationTableName);
            await table.CreateIfNotExistsAsync();
            var copy = results.ToArray(); // have to do this because of batching
            var groups = copy.GroupBy(x => x.PartitionKey);

            foreach (var g in groups)
            {
                var list = new List<TableBatchOperation>();
                var batchOperation = new TableBatchOperation();
                foreach (var result in g)
                {
                    if (batchOperation.Count >= 40)
                    {
                        list.Add(batchOperation);
                        batchOperation = new TableBatchOperation();
                    }
                    batchOperation.Add(TableOperation.InsertOrReplace(result));
                }

                list.Add(batchOperation);
                foreach (var batch in list)
                {
                    await table.ExecuteBatchAsync(batch);
                }
            }
        }
    }
}

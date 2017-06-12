﻿using Autofac;
using Core;
using SampleApplication.Models;
using SQLite;
using System;
using System.Threading.Tasks;

namespace SampleApplication
{
    public interface IRepository
    {
        Task<FetchModelResult<SampleItem>> FetchSampleItemAsync(string id);

        Task<FetchModelCollectionResult<SampleItem>> FetchSampleItemsAsync();

        Task<FetchModelResult<HealthCareUser>> GetCurrentUserAsync();

        Task InitializeAsync();

        Task<Notification> SaveCurrentUserAsync(HealthCareUser user);

        Task<Notification> SaveSampleItemAsync(SampleItem item, ModelUpdateEvent updateEvent);
    }

    public class Repository : IRepository
    {
        #region IRepository implementation

        private readonly string _currentUserId = Guid.NewGuid().ToString();
        private SQLiteAsyncConnection _database;

        private bool _isInitialized = false;

        public async Task<FetchModelResult<SampleItem>> FetchSampleItemAsync(string id)
        {
            FetchModelResult<SampleItem> retResult = new FetchModelResult<SampleItem>();

            var item = await _database.FindAsync<SampleItem>(id);
            retResult.Model = item;

            return retResult;
        }

        public async Task<FetchModelCollectionResult<SampleItem>> FetchSampleItemsAsync()
        {
            FetchModelCollectionResult<SampleItem> retResult = new FetchModelCollectionResult<SampleItem>();
            var items = await _database.Table<SampleItem>().ToListAsync();
            retResult.ModelCollection = items;
            return retResult;
        }

        public async Task<FetchModelResult<HealthCareUser>> GetCurrentUserAsync()
        {
            var retResult = new FetchModelResult<HealthCareUser>();

            var item = await _database.FindAsync<HealthCareUser>(_currentUserId);
            retResult.Model = item;

            return retResult;
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            var connectionFactory = CC.IoC.Resolve<IDatabaseConnectionFactory>();
            var connectionResult = connectionFactory.Execute(null);
            if (connectionResult.IsValid())
            {
                _database = connectionResult.Connection;
                await _database.CreateTableAsync<SampleItem>();
                await _database.CreateTableAsync<HealthCareUser>();

                await CheckCurrentUser();
            }
        }

        public async Task<Notification> SaveCurrentUserAsync(HealthCareUser item)
        {
            Notification retNotification = Notification.Success();
            try
            {
                await _database.UpdateAsync(item);
            }
            catch (SQLiteException)
            {
                retNotification.Add(new NotificationItem("Save Failed"));
            }

            return retNotification;
        }

        public async Task<Notification> SaveSampleItemAsync(SampleItem item, ModelUpdateEvent updateEvent)
        {
            Notification retNotification = Notification.Success();
            try
            {
                if (updateEvent == ModelUpdateEvent.Created)
                {
                    await _database.InsertAsync(item);
                }
                else
                {
                    await _database.UpdateAsync(item);
                }
            }
            catch (SQLiteException)
            {
                //LOG:
                retNotification.Add(new NotificationItem("Save Failed"));
            }

            return retNotification;
        }

        private async Task CheckCurrentUser()
        {
            var result = await GetCurrentUserAsync();
            if (result.Model == null)
            {
                var currentUser = new HealthCareUser { Id = _currentUserId };
                await _database.InsertAsync(currentUser);
            }
        }

        #endregion IRepository implementation
    }
}
﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using WorkLifeBalance.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkLifeBalance.HandlerClasses
{
    public static class DataBaseHandler // add Backup and LoadBackup
    {
        //semaphore to ensure only one method can write to the database at once
        private static SemaphoreSlim _semaphore = new(1);
        //connection string for the db
        private static readonly string ConnectionString = @$"Data Source={Directory.GetCurrentDirectory()}\RecordedData.db;Version=3;";

        public static async Task WriteSettings(WLBSettings sett)
        {
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //open connection
                    await connection.OpenAsync();
                    //if no one writes continue
                    await Task.Run(async () =>
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                string sql = @"UPDATE Settings 
                                              SET LastTimeOpened = @LastTimeOpened,
                                              StartWithWindows = @StartWithWindows,
                                              StartUpCorner = @StartUpCorner,
                                              SaveInterval = @SaveInterval
                                              LIMIT 1";

                                connection.Execute(sql, sett);
                                await transaction.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                //rols back if failed
                                await transaction.RollbackAsync();
                                MainWindow.MainDispatcher.Invoke(() =>
                                {
                                    MainWindow.ShowErrorBox("Failed to write to database", $"This can be caused by a missing database file: {ex.Message}");
                                });
                            }
                        }
                    });

                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //free semaphore so other methods can run
                _semaphore.Release();
            }
        }

        public static async Task<WLBSettings> ReadSettings()
        {
            WLBSettings retrivedSettings = new();
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();
            //if no one writes to db continue

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //start connection
                    await connection.OpenAsync();

                    //gets total number
                    try
                    {
                        string sql = @$"SELECT * FROM Settings
                                        LIMIT 1";

                        retrivedSettings = connection.QueryFirstOrDefault<WLBSettings>(sql);
                    }
                    catch (Exception ex)
                    {
                        MainWindow.MainDispatcher.Invoke(() =>
                        {
                            MainWindow.ShowErrorBox("Failed to write to database", $"This can be caused by a missing database file: {ex.Message}");
                        });
                    }
                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //release sempahore so other methods can run
                _semaphore.Release();
            }

            //returns count
            return retrivedSettings;
        }

        public static async Task WriteDay(DayData day)
        {
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //open connection
                    await connection.OpenAsync();
                    //if no one writes continue
                    await Task.Run(async () =>
                    {
                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                //write data
                                string sql = @"INSERT OR REPLACE INTO Days (Date,WorkedAmmount,RestedAmmount,StudiedAmmount)
                                             VALUES (@Date,@WorkedAmmount,@RestedAmmount,@StudiedAmmount)";

                                await connection.ExecuteAsync(sql, day);
                                await transaction.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                //rols back if failed
                                await transaction.RollbackAsync();
                                MainWindow.MainDispatcher.Invoke(() => 
                                {
                                    MainWindow.ShowErrorBox("Failed to write to database",$"This can be caused by a missing database file: {ex.Message}");
                                });
                            }
                        }
                    });

                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //free semaphore so other methods can run
                _semaphore.Release();
            }
        }

        public static async Task<DayData> ReadDay(string Date)
        {
            DayData? retrivedDay = null;
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();
            //if no one writes to db continue

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //start connection
                    await connection.OpenAsync();

                    //execute the sql to get the day
                    try
                    {
                        string sql = @$"SELECT * FROM Days 
                                      WHERE Date = '{Date}'";
                        retrivedDay = connection.QueryFirstOrDefault<DayData>(sql);
                    }
                    catch (Exception ex)
                    {
                        //close app if failed
                        MainWindow.MainDispatcher.Invoke(() =>
                        {
                            MainWindow.ShowErrorBox("Failed to read from database", $"This can be caused by a missing database file: {ex.Message}");
                        });
                    }
                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //release sempahore so other methods can run
                _semaphore.Release();
            }

            if(retrivedDay == null)
            {
                retrivedDay = new();
            }

            return retrivedDay;
        }

        public static async Task<List<DayData>> ReadMonth(int Month,int year)
        {
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();
            //if no one writes continue

            //list of filtered residences
            List<DayData> ReturnDays = new();

            try
            {
                //write to console it started reading database
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //open connection
                    await connection.OpenAsync();

                    try
                    {
                        //depending on passed arguments chose between 2 sql statements
                        string sql = @$"SELECT * from Days 
                                        WHERE Date Like '%{Month}{year}'";


                        //wait for return, and pass the return to a Residence class
                        var res = await connection.QueryAsync<DayData>(sql);
                        //set the return as a list
                        ReturnDays = res.ToList();

                    }
                    catch (Exception ex)
                    {
                        //close app if failed
                        MainWindow.MainDispatcher.Invoke(() =>
                        {
                            MainWindow.ShowErrorBox("Failed to read from database", $"This can be caused by a missing database file: {ex.Message}");
                        });
                    }
                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //release semaphore so other methods could run
                _semaphore.Release();
            }
            //return filtered res
            return ReturnDays;
        }

        public static async Task<DayData> GetMaxValue(string Value,int Month = -1, int year = -1)
        {
            DayData? retrivedDay = null;
            //wait for a time when no methods writes now to the database
            await _semaphore.WaitAsync();
            //if no one writes to db continue

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
                {
                    //start connection
                    await connection.OpenAsync();

                    //execute the sql to get the day
                    try
                    {
                        string sql;
                        if (Month == -1 || year == -1)
                        {
                            sql = @$"SELECT * FROM Days 
                                     WHERE {Value} = 
                                    (SELECT MAX({Value}) FROM Days)";
                        }
                        else
                        {

                            sql = @$"SELECT * FROM Days 
                                     WHERE {Value} = 
                                     (SELECT MAX({Value}) FROM Days
                                     WHERE Date Like '{Month}%{year}')";
                        }
                        retrivedDay = connection.QueryFirstOrDefault<DayData>(sql);
                    }
                    catch (Exception ex)
                    {
                        //close app if failed
                        MainWindow.MainDispatcher.Invoke(() =>
                        {
                            MainWindow.ShowErrorBox("Failed to read from database", $"This can be caused by a missing database file: {ex.Message}");
                        });
                    }
                    //close connection
                    await connection.CloseAsync();
                }
            }
            finally
            {
                //release sempahore so other methods can run
                _semaphore.Release();
            }

            if (retrivedDay == null)
            {
                Console.WriteLine("Day returned NUlld");
                Console.WriteLine(Value);
                Console.WriteLine(year);
                Console.WriteLine(Month);
                retrivedDay = new();
            }

            return retrivedDay;
        }

    }
}
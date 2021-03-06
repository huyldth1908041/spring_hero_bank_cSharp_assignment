﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using spring_hero_bank_cSharp_assignment.Controller;
using spring_hero_bank_cSharp_assignment.Entity;

namespace spring_hero_bank_cSharp_assignment.Model
{
    public class ShbTransactionModel
    {
        public List<SHBTransaction> GetTransactionsByAccountNumber(string accountNumber)
        {
            List<SHBTransaction> listTransactions = new List<SHBTransaction>();
            var connection = ConnectionHelper.GetConnection();
            try
            {
                connection.Open();
                string getTransactionStringCmd =
                    $"SELECT * FROM `shb-transactions` WHERE senderAccountNumber = '{accountNumber}'";
                MySqlCommand getTransactionSqlCommand = new MySqlCommand(getTransactionStringCmd, connection);
                var transactionReader = getTransactionSqlCommand.ExecuteReader();
                while (transactionReader.Read())
                {
                    var shbTransaction = new SHBTransaction()
                    {
                        Code = transactionReader.GetString("code"),
                        SenderAccountNumber = transactionReader.GetString("senderAccountNumber"),
                        ReceiverAccountNumber = transactionReader.GetString("receiverAccountNumber"),
                        Message = transactionReader.GetString("message"),
                        Amount = transactionReader.GetDouble("amount"),
                        Fee = transactionReader.GetDouble("fee"),
                        CreateAt = transactionReader.GetDateTime("createAt"),
                        UpdateAt = transactionReader.GetDateTime("updateAt"),
                        Status = (TransactionStatus) transactionReader.GetInt32("status"),
                        Type = (TransactionType) transactionReader.GetInt32("type")
                    };
                    listTransactions.Add(shbTransaction);
                }

                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("lỗi khi lấy danh sách giao dịch " + e.Message);
                connection.Close();
                return null;
            }

            return listTransactions;
        }

        public List<SHBTransaction> GetListTransactions()
        {
            List<SHBTransaction> listTransactions = new List<SHBTransaction>();
            var connection = ConnectionHelper.GetConnection();
            try
            {
                connection.Open();
                string getTransactionStringCmd = "SELECT * FROM `shb-transactions`;";
                MySqlCommand getTransactionSqlCommand = new MySqlCommand(getTransactionStringCmd, connection);
                var transactionReader = getTransactionSqlCommand.ExecuteReader();
                while (transactionReader.Read())
                {
                    var shbTransaction = new SHBTransaction()
                    {
                        Code = transactionReader.GetString("code"),
                        SenderAccountNumber = transactionReader.GetString("senderAccountNumber"),
                        ReceiverAccountNumber = transactionReader.GetString("receiverAccountNumber"),
                        Message = transactionReader.GetString("message"),
                        Amount = transactionReader.GetDouble("amount"),
                        Fee = transactionReader.GetDouble("fee"),
                        CreateAt = transactionReader.GetDateTime("createAt"),
                        UpdateAt = transactionReader.GetDateTime("updateAt"),
                        Status = (TransactionStatus) transactionReader.GetInt32("status"),
                        Type = (TransactionType) transactionReader.GetInt32("type")
                    };
                    listTransactions.Add(shbTransaction);
                }

                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("lỗi khi lấy danh sách giao dịch " + e.Message);
                connection.Close();
                return null;
            }

            return listTransactions;
        }
    }
}
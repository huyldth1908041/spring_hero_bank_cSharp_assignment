﻿using System;
using System.Collections.Generic;
using spring_hero_bank_cSharp_assignment.Entity;
using spring_hero_bank_cSharp_assignment.Helper;
using spring_hero_bank_cSharp_assignment.Model;
using spring_hero_bank_cSharp_assignment.View;

namespace spring_hero_bank_cSharp_assignment.Controller
{
    public class AccountController
    {
        private PasswordHelper _passwordHelper = new PasswordHelper();
        private static AccountModel _accountModel = new AccountModel();
        private ShbTransactionModel _transactionModel = new ShbTransactionModel();


        public Account Login() // Đăng nhập hệ thống 
        {
            Console.WriteLine("Nhập tên tài khoản: ");
            var username = Console.ReadLine();
            Console.WriteLine("Nhập mật khẩu: ");
            var password = PromptHelper.GetPassword();
            var account = _accountModel.GetActiveAccountByUsername(username);
            // mã hóa pass người dùng nhập vào kèm theo muối trong database và so sánh kết quả với password đã được mã hóa trong database.
            if (account != null && _passwordHelper.ComparePassword(password, account.Salt, account.PasswordHash))
            {
                Console.WriteLine("Đăng nhập thành công");
                return account;
            }

            Console.WriteLine("đăng nhập thất bại: thông tin tài khoản hoặc mật khẩu không đúng");
            return null;
        }

        //TODO: refactor
        // 1. Danh sách người dùng
        public void ListAccount()
        {
            foreach (var account in _accountModel.GetListAccount())
            {
                Console.WriteLine(account.ToString());
            }
        }


        // 3. Tìm kiếm người dùng theo tên.
        public Account SearchAccountByName()
        {
            Console.WriteLine("Tìm kiếm người dùng theo tên: ");
            var fullName = Console.ReadLine();

            return _accountModel.GetAccountByName(fullName);
        }

        // 4. Tìm kiếm người dùng theo số tài khoản.
        public Account SearchAccountByAccountNumber()
        {
            Console.WriteLine("Tìm kiếm người dùng theo số tài khoản: ");
            var accountNumber = Console.ReadLine();
            //TODO: fix this
            return _accountModel.GetAccountByAccountNumber(accountNumber);
        }

        // 5. Tìm kiếm người dùng theo số điện thoại
        public Account SearchAccountByPhoneNumber()
        {
            Console.WriteLine("Tìm kiếm người dùng theo số điện thoại: ");
            var phoneNumber = Console.ReadLine();
            var account = _accountModel.GetAccountByPhoneNumber(phoneNumber);

            return account; //TODO: check null
        }

        // 6. Thêm người dùng mới
        public void AddUser()
        {
            string accountNumber;
            while (true)
            {
                accountNumber = AccountHelper.RandomAccountNumber(15);
                var isExist = _accountModel.CheckExistAccountByUsername(accountNumber);
                if (isExist == false)
                {
                    break;
                }
            }

            var newAccount = new Account()
            {
                Balance = 0,
                Status = AccountStatus.ACTIVE,
                Salt = _passwordHelper.GenerateSalt(),
                AccountNumber = accountNumber,
                Role = AccountRole.GUEST
            };

            Console.WriteLine("--Đăng kí--"); //tieng viet
            Console.WriteLine("Nhập tên đăng nhập");
            string username = Console.ReadLine();
            while (_accountModel.CheckExistAccountByUsername(username))
            {
                Console.WriteLine("Tên đăng nhập đã tồn tại vui long chon tên đăng nhập khác");
                username = Console.ReadLine();
            }

            newAccount.Username = username;

            Console.WriteLine("Enter password");
            var password = Console.ReadLine();

            newAccount.PasswordHash = _passwordHelper.MD5Hash(newAccount.Salt + password);

            Console.WriteLine("Enter your full name");
            newAccount.FullName = Console.ReadLine();

            Console.WriteLine("Enter your email");
            newAccount.Email = Console.ReadLine();

            Console.WriteLine("Enter your phone number");
            newAccount.PhoneNumber = Console.ReadLine();

            _accountModel.SaveAccount(newAccount);
        }

        // 7. Khoá và mở tài khoản người dùng
        // 7.1. Khóa tài khoản người dùng


        public bool LockAccount()
        {
            Console.WriteLine("Nhập vào số tài khoản bạn muốn khóa");
            var toLockAccountNumber = Console.ReadLine();
            var isSuccess = _accountModel.UpdateAccountStatusByAccountNumber(toLockAccountNumber, AccountStatus.LOCKED);
            if (isSuccess)
            {
                Console.WriteLine($"Đã khóa tài khoản với số tài khoản {toLockAccountNumber} thành công");
                return true;
            }

            Console.WriteLine("Khóa tài khoản " + toLockAccountNumber + " thất bại !");
            return false;
        }

        // 7.2. Mở tài khoản người dùng
        public bool UnLockAccount()
        {
            Console.WriteLine("Nhập vào số tài khoản muốn mở khóa");
            var accountNumber = Console.ReadLine();
            var isSuccess = _accountModel.UpdateAccountStatusByAccountNumber(accountNumber, AccountStatus.ACTIVE);
            if (isSuccess)
            {
                Console.WriteLine($"Đã mở khóa tài khoản với số tài khoản {accountNumber} thành công !");
                return true;
            }

            Console.WriteLine($"mở khóa tài khoản {accountNumber} thất bại !");
            return false;
        }

        public List<SHBTransaction> GetTransactionsByAccountNumber(string accountNumber)
        {
            return _transactionModel.GetTransactionsByAccountNumber(accountNumber);
        }


        public Account Register()
        {
            var accountNumber = "";
            while (true)
            {
                accountNumber = AccountHelper.RandomAccountNumber(15);
                var isExist = _accountModel.CheckExistAccountNumber(accountNumber);
                if (!isExist)
                {
                    break;
                }
            }

            //init account obj 
            var newAccount = new Account()
            {
                Balance = 0,
                Status = AccountStatus.ACTIVE,
                Salt = _passwordHelper.GenerateSalt(),
                AccountNumber = accountNumber,
                Role = AccountRole.GUEST
            };
            //get full Name
            Console.WriteLine("Nhập tên đầy đủ: ");
            string fullName = Console.ReadLine();
            newAccount.FullName = fullName;
            //get email
            Console.WriteLine("Nhập email của bạn: ");
            string email;
            while (true)
            {
                email = Console.ReadLine();
                if (ValidateHelper.IsEmailValid(email))
                {
                    break;
                }

                Console.WriteLine("email không hợp lệ mời nhập lại");
            }

            newAccount.Email = email;
            //get phone number
            Console.WriteLine("Nhập số điện thoại: ");
            string phoneNumber;
            while (true)
            {
                phoneNumber = Console.ReadLine();
                if (ValidateHelper.IsPhoneNumberValid(phoneNumber))
                {
                    break;
                }

                Console.WriteLine("Số điện thoại không hợp lệ mời nhập lại");
            }

            newAccount.PhoneNumber = phoneNumber;
            //get username
            Console.WriteLine("Nhập tên đăng nhập: ");
            //prompt for username if exsist -> prompt again
            string username;
            while (true)
            {
                username = Console.ReadLine();
                var isValid = ValidateHelper.IsUsernameValid(username);
                var isExist = _accountModel.CheckExistAccountByUsername(username);
                if (isValid && !isExist)
                {
                    break;
                }

                Console.WriteLine("Tên đăng nhập không hợp lệ hoặc đã tồn tại hãy nhập lại");
            }

            newAccount.Username = username;
            //get password
            Console.WriteLine("Hãy nhập vào mật khẩu của bạn");
            string password;
            while (true)
            {
                password = PromptHelper.GetPassword();
                if (ValidateHelper.IsPasswordValid(password))
                {
                    break;
                }

                Console.WriteLine("Mật khẩu không hợp lệ mời nhập lại");
            }

            //hash pwd and salt to get password hashed
            newAccount.PasswordHash = _passwordHelper.MD5Hash(password + newAccount.Salt);

            var result = _accountModel.SaveAccount(newAccount);
            if (result == false)
            {
                return null;
            }
            else
            {
                return newAccount;
            }
        }

        public bool UpdatePhoneNumber(string accountNumber)
        {
            Console.WriteLine("Số điện thoại hiện tại: " + ConsoleView.CurrentLogin.PhoneNumber);
            Console.WriteLine("Nhập số điện thoại mới của bạn: ");
            string newPhoneNumber;
            while (true)
            {
                newPhoneNumber = Console.ReadLine();
                if (ValidateHelper.IsPhoneNumberValid(newPhoneNumber))
                {
                    break;
                }

                Console.WriteLine("Số điện thoại không hợp lệ hãy nhập lại");
            }

            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "phoneNumber", newPhoneNumber);
            if (res == true)
            {
                Console.WriteLine($"Đã update số điện thoại của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin thất bại");
                return false;
            }

            return false;
        }

        public bool UpdateFullName(string accountNumber)
        {
            Console.WriteLine("Tên hiện tại: " + ConsoleView.CurrentLogin.FullName);
            Console.WriteLine("Nhập tên đầy đủ mới của bạn");
            string newFullName = Console.ReadLine();

            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "fullName", newFullName);
            if (res == true)
            {
                Console.WriteLine($"Đã update tên của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin thất bại");
                return false;
            }

            return false;
        }

        public bool UpdateEmail(string accountNumber)
        {
            Console.WriteLine("Email hiện tại " + ConsoleView.CurrentLogin.Email);
            Console.WriteLine("Nhập email mới của bạn");
            string newEmail = "";
            while (true)
            {
                newEmail = Console.ReadLine();
                if (ValidateHelper.IsEmailValid(newEmail))
                {
                    break;
                }

                Console.WriteLine("Email không hợp lệ mời nhập lại");
            }

            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "email", newEmail);
            if (res == true)
            {
                Console.WriteLine($"Đã update email của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin thất bại");
                return false;
            }

            return false;
        }


        public bool UpdatePassWord(string accountNumber)
        {
            var account = _accountModel.GetAccountByAccountNumber(accountNumber);
            Console.WriteLine("Nhập mật khẩu cũ của bạn: ");
            string oldPassWord = PromptHelper.GetPassword();

            // mã hóa pass người dùng nhập vào kèm theo muối trong database và so sánh kết quả với password đã được mã hóa trong database.
            if (account != null && _passwordHelper.ComparePassword(oldPassWord, account.Salt, account.PasswordHash))
            {
                string newPassword;
                while (true)
                {
                    Console.WriteLine("nhập mật khẩu mới");
                    newPassword = PromptHelper.GetPassword();
                    if (!ValidateHelper.IsPasswordValid(newPassword))
                    {
                        Console.WriteLine("Mật khẩu không hợp lệ mời nhập lại");
                    }
                    else if (oldPassWord.Equals(newPassword))
                    {
                        Console.WriteLine("Bạn đã nhập mật khẩu cũ mời nhập lại");
                    }
                    else
                    {
                        break;
                    }
                }

                //update md5hash
                string newHashPassWord = _passwordHelper.MD5Hash(newPassword + account.Salt);
                _accountModel.UpdateAccountByAccountNumber(accountNumber, "hashPassword", newHashPassWord);
                Console.WriteLine("Đã cập nhật mật khẩu thành công");
                return true;
            }

            Console.WriteLine("Bạn đã nhập sai mật khẩu hãy thử lại");
            return false;
        }

        public bool Deposit(string accountNumber)
        {
            Console.WriteLine("Nhập số tiền bạn muốn gửi: ");
            double amount;
            while (true)
            {
                amount = double.Parse(Console.ReadLine());
                if (amount > 0)
                {
                    break;
                }

                Console.WriteLine("số tiền không họp lệ mời nhập lại");
            }

            if (_accountModel.Deposit(accountNumber, amount))
            {
                Console.WriteLine($"Đã gửi {amount}đ thành công vào tài khoản {accountNumber} phí giao dịch 1100đ");
                Console.WriteLine("Số dư tại thời điểm giao dịch: " +
                                  _accountModel.GetCurrentBalanceByAccountNumber(accountNumber));
                return true;
            }

            return false;
        }

        public bool WithDraw(string accountNumber)
        {
            Console.WriteLine("Nhập số tiền bạn muốn rút: ");
            double amount;
            while (true)
            {
                amount = double.Parse(Console.ReadLine());
                if (amount > 0)
                {
                    break;
                }

                Console.WriteLine("số tiền không họp lệ mời nhập lại");
            }

            if (_accountModel.Withdraw(accountNumber, amount))
            {
                Console.WriteLine($"Đã rút {amount} thành công từ tài khoản {accountNumber} phí giao dịch 1100đ");
                Console.WriteLine("Số dư tại thời điểm giao dịch: " +
                                  _accountModel.GetCurrentBalanceByAccountNumber(accountNumber));
                return true;
            }

            return false;
        }

        public bool Transfer(string senderAccountNumber)
        {
            bool result = false;
            Console.WriteLine("Nhập số tài khoản người nhận: ");
            string receiverAccountNumber = Console.ReadLine();
            if (receiverAccountNumber.Equals(senderAccountNumber))
            {
                Console.WriteLine("không thể chuyển khoản cho cùng 1 số tài khoản");
                return false;
            }

            var receiverAccount = _accountModel.GetAccountByAccountNumber(receiverAccountNumber);
            if (receiverAccount == null)
            {
                return false;
            }

            Console.WriteLine(
                "---------------------------------------------------------------------------------------");
            Console.WriteLine(
                "                              Thông tin người nhận                                     ");
            Console.WriteLine(
                "---------------------------------------------------------------------------------------");
            Console.WriteLine("TÊN: " + receiverAccount.FullName);
            Console.WriteLine("EMAIL: " + receiverAccount.Email);
            Console.WriteLine("SỐ ĐIỆN THOẠI: " + receiverAccount.PhoneNumber);
            Console.WriteLine(
                "---------------------------------------------------------------------------------------");
            Console.WriteLine("Bạn có muốn chuyển khoản cho người này ?");
            Console.WriteLine("1. Có");
            Console.WriteLine("2. Không");
            Console.WriteLine(
                "---------------------------------------------------------------------------------------");
            Console.WriteLine("Nhập lựa chọn của bạn");
            var choice = PromptHelper.GetUserChoice(1, 2);
            switch (choice)
            {
                case 1:
                    Console.WriteLine("Nhập số tiền bạn muốn chuyển khoản: ");
                    double amount;
                    while (true)
                    {
                        amount = double.Parse(Console.ReadLine());
                        if (amount > 0)
                        {
                            break;
                        }

                        Console.WriteLine("số tiền không họp lệ mời nhập lại");
                    }

                    result = _accountModel.Transfer(senderAccountNumber, receiverAccountNumber, amount);
                    break;
                case 2:
                    Console.WriteLine("Quay lại menu chính...");
                    result = true;
                    break;
                default:
                    Console.WriteLine("lựa chọn k hợp lệ");
                    result = false;
                    break;
            }

            if (result == true)
            {
                Console.WriteLine("Chuyển khoản thành công số dư tài khoản tại thời điểm giao dịch " +
                                  _accountModel.GetCurrentBalanceByAccountNumber(senderAccountNumber));
            }

            return result;
        }

        public double GetBalance(string accountNumber)
        {
            return _accountModel.GetCurrentBalanceByAccountNumber(accountNumber);
        }
    }
}
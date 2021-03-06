﻿using System;
using System.Collections.Generic;
using spring_hero_bank_cSharp_assignment.Entity;
using spring_hero_bank_cSharp_assignment.Helper;
using spring_hero_bank_cSharp_assignment.Model;

namespace spring_hero_bank_cSharp_assignment.Controller
{
    public class AccountController
    {
        private PasswordHelper _passwordHelper = new PasswordHelper();
        private static AccountModel _accountModel = new AccountModel();
        private ShbTransactionModel _transactionModel = new ShbTransactionModel();


        public Account Login() 
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

            return null;
        }
        
        public List<Account> ListAccount()
        {
            var listAccount = _accountModel.GetListAccount();
            return listAccount;
        }
        
        public List<SHBTransaction> GetListTransactions()
        {
            var listTransactions = _transactionModel.GetListTransactions();
            return listTransactions;
        }
        
        public List<Account> SearchAccountByName()
        {
            Console.WriteLine("Nhập tên tài khoản muốn tìm: ");
            var fullName = Console.ReadLine();
            return _accountModel.GetAccountsByName(fullName);
        }
        
        public Account SearchAccountByAccountNumber()
        {
            Console.WriteLine("Nhập số tài khoản muốn tìm kiếm: ");
            var accountNumber = Console.ReadLine();
            return _accountModel.GetAccountByAccountNumber(accountNumber);
        }
        
        public Account SearchAccountByPhoneNumber()
        {
            Console.WriteLine("Nhập số điện thoại muốn tìm: ");
            var phoneNumber = Console.ReadLine();
            var account = _accountModel.GetAccountByPhoneNumber(phoneNumber);
            return account;
        }
        
        public Account AddUser() // cho phép thêm tài khoản khách hoặc admin
        {
            var accountNumber = "";
            while (true)
            {
                accountNumber = AccountHelper.RandomAccountNumber(15);
                var isExist = _accountModel.CheckExistAccountNumber(accountNumber);
                if (isExist == null)
                {
                    Console.WriteLine("Kiểm tra kết nối của bạn");
                    break;
                }

                if (isExist == false)
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
            };
            //get Role
            Console.WriteLine("Bạn muốn tạo tài khoản thường hay tài khoản cho admin ?");
            Console.WriteLine("1. Tài khoản thường");
            Console.WriteLine("2. Tài khoản admin");
            Console.WriteLine("Chọn 1 hoặc 2");
            var choice = PromptHelper.GetUserChoice(1, 2);
            switch (choice)
            {
                case 1:
                    newAccount.Role = AccountRole.GUEST;
                    break;
                case 2:
                    newAccount.Role = AccountRole.ADMIN;
                    break;
            }

            //get full Name
            Console.WriteLine("Nhập tên đầy đủ: ");
            string fullName = Console.ReadLine();
            newAccount.FullName = fullName;
            //get email
            Console.WriteLine("Nhập email: ");
            string email;
            while (true)
            {
                email = Console.ReadLine();
                if (ValidateHelper.IsEmailValid(email))
                {
                    break;
                }

                Console.WriteLine("Email không hợp lệ mời nhập lại");
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
                if (isExist == null)
                {
                    Console.WriteLine("Hãy kiểm tra kết nối của bạn");
                    return null;
                }
                if (isValid && isExist == false)
                {
                    break;
                }

                if (!isValid)
                {
                    Console.WriteLine("tên đăng nhập không hợp lệ mời nhập lại");
                    continue;
                }

                if (isExist == true)
                {
                    Console.WriteLine("Tên đăng nhập đã tồn tại mời nhập lại");
                    continue;
                }
                
            }

            newAccount.Username = username;
            //get password
            Console.WriteLine("Hãy nhập vào mật khẩu:");
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

            //xác nhận mật khẩu
            Console.WriteLine("Nhập lại mật khẩu của bạn để xác nhận: ");
            string confirmPass = "";
            while (true)
            {
                confirmPass = PromptHelper.GetPassword();
                if (confirmPass.Equals(password))
                {
                    break;
                }

                Console.WriteLine("Mật khẩu không khớp, mời nhập lại");
            }


            //hash pwd and salt to get password hashed
            newAccount.PasswordHash = _passwordHelper.MD5Hash(password + newAccount.Salt);

            var result = _accountModel.SaveAccount(newAccount);
            if (result == false)
            {
                return null;
            }

            return newAccount;
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

            Console.WriteLine($"Mở khóa tài khoản {accountNumber} không thành công!");
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
                if (isExist == null)
                {
                    Console.WriteLine("Hãy kiểm tra lại kết nối của bạn");
                    return null;
                }

                if (isExist == false)
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

                Console.WriteLine("Email không hợp lệ mời nhập lại");
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
                if (isExist == null)
                {
                    Console.WriteLine("Hãy kiểm tra lại kết nối của bạn");
                    return null;
                }

                if (isExist == false && isValid)
                {
                    break;
                }
                if (isExist == true)
                {
                    Console.WriteLine("Tên đăng nhập đã tồn tại");
                    continue;
                }

                if (isValid == false)
                {
                    Console.WriteLine("Tên đăng nhập không hợp lệ ");
                    continue;
                }
             
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

            //xác nhận mật khẩu
            Console.WriteLine("Nhập lại mật khẩu của bạn để xác nhận: ");
            string confirmPass = "";
            while (true)
            {
                confirmPass = PromptHelper.GetPassword();
                if (confirmPass.Equals(password))
                {
                    break;
                }

                Console.WriteLine("Mật khẩu không khớp, mời nhập lại");
            }


            //hash pwd and salt to get password hashed
            newAccount.PasswordHash = _passwordHelper.MD5Hash(password + newAccount.Salt);

            var result = _accountModel.SaveAccount(newAccount);
            if (result == false)
            {
                return null;
            }

            return newAccount;
        }

        public bool UpdatePhoneNumber(string accountNumber)
        {
            var currentAccount = _accountModel.GetAccountByAccountNumber(accountNumber);
            var oldPhone = currentAccount.PhoneNumber;
            Console.WriteLine("Số điện thoại hiện tại là: " + oldPhone);
            Console.WriteLine("Nhập số điện thoại mới của bạn: ");
            string newPhoneNumber;
            while (true)
            {
                newPhoneNumber = Console.ReadLine();
                if (ValidateHelper.IsPhoneNumberValid(newPhoneNumber) && !newPhoneNumber.Equals(oldPhone))
                {
                    break;
                }

                if (!ValidateHelper.IsEmailValid(newPhoneNumber))
                {
                    Console.WriteLine("Số điện thoại không hợp lệ hãy nhập lại");
                    continue;
                }

                if (newPhoneNumber.Equals(oldPhone))
                {
                    Console.WriteLine("Bạn đã nhập vào số điện thoại cũ mời nhập lại");
                }
            }

            //confirm update
            string confirmMessage = $"Bạn có muốn lưu số điện thoại {newPhoneNumber} không ? ";
            var isConfirm = PromptHelper.ConfirmUser(confirmMessage);
            if (isConfirm == false)
            {
                return false;
            }
            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "phoneNumber", newPhoneNumber);
            if (res == true)
            {
                Console.WriteLine($"Đã update số điện thoại của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin không thành công");
                return false;
            }

            return false;
        }

        public bool UpdateFullName(string accountNumber)
        {
            var currentAccount = _accountModel.GetAccountByAccountNumber(accountNumber);
            var oldName = currentAccount.FullName;
            Console.WriteLine("Tên hiện tại: " + oldName);
            Console.WriteLine("Nhập tên đầy đủ mới của bạn");
            string newFullName;
            while (true)
            {
                newFullName = Console.ReadLine();
                if (!newFullName.Equals(oldName))
                {
                    break;
                }

                Console.WriteLine("Bạn đã nhập tên cũ mời bạn nhập lại");
            }

            //confirm update
            string confirmMessage = $"Bạn có muốn lưu tên {newFullName} không ? ";
            var isConfirm = PromptHelper.ConfirmUser(confirmMessage);
            if (isConfirm == false)
            {
                return false;
            }
            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "fullName", newFullName);
            if (res == true)
            {
                Console.WriteLine($"Đã update tên của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin không thành công");
                return false;
            }

            return false;
        }

        public bool UpdateEmail(string accountNumber)
        {
            var currentAccount = _accountModel.GetAccountByAccountNumber(accountNumber);
            var oldEmail = currentAccount.Email;
            Console.WriteLine("Email hiện tại " + oldEmail);
            Console.WriteLine("Nhập email mới của bạn");
            string newEmail = "";
            while (true)
            {
                newEmail = Console.ReadLine();
                if (ValidateHelper.IsEmailValid(newEmail) && !newEmail.Equals(oldEmail))
                {
                    break;
                }

                if (!ValidateHelper.IsEmailValid(newEmail))
                {
                    Console.WriteLine("Email không hợp lệ mời nhập lại");
                    continue;
                }

                if (newEmail.Equals(oldEmail))
                {
                    Console.WriteLine("Bạn đã nhập email cũ, mời nhập lại");
                }
            }
            //confirm update
            var isConfirm = PromptHelper.ConfirmUser($"Bạn có muốn thay đổi email thành {newEmail} hay không ?");
            if (isConfirm == false)
            {
                return false;
            }

            var res = _accountModel.UpdateAccountByAccountNumber(accountNumber, "email", newEmail);
            if (res == true)
            {
                Console.WriteLine($"Đã update email của số tài khoản {accountNumber} thành công");
            }
            else
            {
                Console.WriteLine("Update thông tin không thành công");
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

                //confirm new password
                Console.WriteLine("Hãy nhập lại mật khẩu để xác nhận");
                string confirmNewPassword = "";
                while (true)
                {
                    confirmNewPassword = PromptHelper.GetPassword();
                    if (confirmNewPassword.Equals(newPassword))
                    {
                        break;
                    }

                    Console.WriteLine("Mật khẩu không khớp, hãy nhập lại");
                }

                //update md5hash
                string newHashPassWord = _passwordHelper.MD5Hash(newPassword + account.Salt);
                var updateSuccess = _accountModel.UpdateAccountByAccountNumber(accountNumber, "hashPassword", newHashPassWord);
                if (updateSuccess)
                {
                    Console.WriteLine("Đã cập nhật mật khẩu thành công");
                    return true;
                }
                return false;
            }

            Console.WriteLine("Bạn đã nhập sai mật khẩu");
            return false;
        }

        public bool Deposit(string accountNumber)
        {
            Console.WriteLine("Nhập số tiền muốn gửi: ");
            var amount = PromptHelper.GetAmount();
            //confirm
            string confirmMessage = $"Bạn có muốn gửi {amount} không ? ";
            var isConfirm = PromptHelper.ConfirmUser(confirmMessage);
            if (isConfirm == false)
            {
                return false;
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

            var amount = PromptHelper.GetAmount();
            //confirm 
            string confirmMessage = $"Bạn có muốn rút {amount} không ? ";
            var isConfirm = PromptHelper.ConfirmUser(confirmMessage);
            if (isConfirm == false)
            {
                return false;
            }

            Console.WriteLine();
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
                    var amount = PromptHelper.GetAmount();
                    //confirm transfer
                    
                    result = _accountModel.Transfer(senderAccountNumber, receiverAccountNumber, amount);
                    break;
                case 2:
                    Console.WriteLine("Quay lại menu chính...");
                    result = true;
                    break;
                default:
                    Console.WriteLine("Lựa chọn không hợp lệ");
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
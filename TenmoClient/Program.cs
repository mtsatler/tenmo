using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;


namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        
        private static readonly string API_URL = "https://localhost:44315/api/";

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            //menuselection -1 is to start from beggining of loop
            MenuSelection(-1); 
        }

        //added paramenter to easily move to different menu options from within method
        //pass -1 to restart loop
        private static void MenuSelection(int menuSelection) 
        {

            APIService apiService = new APIService(API_URL);

            while (menuSelection != 0)
            {
                if (menuSelection == -1)
                {

                    Console.WriteLine("");
                    Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                    Console.WriteLine("1: View your current balance"); //done
                    Console.WriteLine("2: View your past transfers"); //done
                    Console.WriteLine("3: View your pending requests"); //TODO
                    Console.WriteLine("4: Send TE bucks"); //done
                    Console.WriteLine("5: Request TE bucks"); //done
                    Console.WriteLine("6: Log in as different user");
                    Console.WriteLine("0: Exit");
                    Console.WriteLine("---------");
                    Console.Write("Please choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out menuSelection))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                        menuSelection = -1;
                    }

                }

                else if (menuSelection == 1)
                {
                    //return current balance of users accounts
                    //currently each user only has one account, future proofed for multiple accounts
                    List<API_Account> accounts = apiService.GetAccountsOfCurrentUser();

                    WriteLineBreak();

                    foreach (API_Account account in accounts)
                    {
                        Console.WriteLine($"Your current account balance is: {account.Balance:C2}");
                    }

                    WriteLineBreak();
                    menuSelection = -1;


                }
                else if (menuSelection == 2)
                {

                    List<API_Transfer> transfers = apiService.GetTransfersOfCurrentUser();

                    if (transfers.Count > 0)
                    {
                        WriteLineBreak();
                        Console.WriteLine("Transfers");
                        Console.WriteLine($"ID {"From/To".PadLeft(10)} {"Amount".PadLeft(18)}");
                        WriteLineBreak();

                        foreach (API_Transfer transfer in transfers)
                        {
                            string toOrFrom = "";
                            string name = "";

                            if (transfer.account_from == UserService.GetUserId())
                            {
                                toOrFrom = "To: ";
                                name = apiService.GetUserNameFromId(transfer.account_to);
                            }
                            else
                            {
                                toOrFrom = "From: ";
                                name = apiService.GetUserNameFromId(transfer.account_from);
                            }

                            Console.WriteLine($"{transfer.transfer_id} {toOrFrom.PadLeft(10)}{name} {" ".PadLeft(10)}{transfer.amount:C2}");

                        }

                        WriteLineBreak();
                        Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                        if (!int.TryParse(Console.ReadLine(), out menuSelection))
                        {
                            Console.WriteLine("Invalid input. Please enter only a number.");
                        }
                        else if (menuSelection == 0)
                        {
                            menuSelection = -1;
                            continue;
                        }
                        else
                        {
                            try
                            {
                                API_Transfer transfer = apiService.GetTransferById(menuSelection);
                                API_TransferStatus transferStatus = apiService.GetTransferStatusById(transfer.transfer_status_id);
                                API_TransferType transferType = apiService.GetTransferTypeById(transfer.transfer_type_id);
                                string fromUser = apiService.GetUserNameFromId(transfer.account_from);
                                string toUser = apiService.GetUserNameFromId(transfer.account_to);

                                WriteLineBreak();
                                Console.WriteLine("Transfer Details");
                                WriteLineBreak();
                                Console.WriteLine($"Id: {transfer.transfer_id}");
                                Console.WriteLine($"From: {fromUser}");
                                Console.WriteLine($"To: {toUser}");
                                Console.WriteLine($"Type: {transferType.transfer_type_desc}");
                                Console.WriteLine($"Status: {transferStatus.transfer_status_desc}");
                                Console.WriteLine($"Amount: {transfer.amount:c2}");
                                WriteLineBreak();

                            }
                            catch (Exception)
                            {
                                menuSelection = -1;
                                Console.WriteLine("Transfer not found");
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("No transfers found");
                    }
                    menuSelection = -1;

                }
                else if (menuSelection == 3)
                {

                    //TODO view pending requests
                    List<API_Transfer> allTransfers = apiService.GetTransfersOfCurrentUser();

                    if (allTransfers.Count > 0)
                    {
                        List<API_Transfer> transfers = new List<API_Transfer>();
                        for (int i = 0; i < allTransfers.Count; i++)
                        {
                            API_TransferStatus transferStatus = apiService.GetTransferStatusById(allTransfers[i].transfer_status_id);
                            API_TransferType transferType = apiService.GetTransferTypeById(allTransfers[i].transfer_type_id);
                            if (transferStatus.transfer_status_desc.ToLower().Equals("pending"))
                            {
                                transfers.Add(allTransfers[i]);
                            }
                        }

                        if (transfers.Count > 0)
                        {
                            List<API_Transfer> temp = new List<API_Transfer>();
                            foreach (API_Transfer transfer in transfers)
                            {
                                try
                                {
                                    API_TransferStatus transferStatus = apiService.GetTransferStatusById(transfer.transfer_status_id);
                                    API_TransferType transferType = apiService.GetTransferTypeById(transfer.transfer_type_id);
                                    string fromUser = apiService.GetUserNameFromId(transfer.account_from);
                                    string toUser = apiService.GetUserNameFromId(transfer.account_to);

                                    string toOrFrom = "";
                                    string name = "";
                                    if (transfer.account_from == UserService.GetUserId())
                                    {
                                        temp.Add(transfer);
                                        if (transfer.account_from == UserService.GetUserId())
                                        {
                                            toOrFrom = "To: ";
                                            name = apiService.GetUserNameFromId(transfer.account_to);
                                        }
                                        else
                                        {
                                            toOrFrom = "From: ";
                                            name = apiService.GetUserNameFromId(transfer.account_from);
                                        }

                                        Console.WriteLine($"{transfer.transfer_id} {toOrFrom.PadLeft(10)}{name} {" ".PadLeft(10)}{transfer.amount:C2}");
                                        WriteLineBreak();

                                        Console.WriteLine("1: Approve");
                                        Console.WriteLine("2: Reject");
                                        Console.WriteLine("0: Don't approve or reject");
                                        WriteLineBreak();
                                        Console.WriteLine("Please choose an option: ");
                                        if (!int.TryParse(Console.ReadLine(), out menuSelection))
                                        {
                                            menuSelection = -1;
                                            Console.WriteLine("Invalid input. Please enter only a number.");
                                        }
                                        else if (menuSelection == 0)
                                        {
                                            menuSelection = -1;
                                            continue;
                                        }
                                        else if (menuSelection == 1)
                                        {
                                            transfer.transfer_status_id = 2;
                                            transferStatus = apiService.GetTransferStatusById(transfer.transfer_status_id);

                                            WriteLineBreak();
                                            Console.WriteLine("Transfer Details");
                                            WriteLineBreak();
                                            Console.WriteLine($"Id: {transfer.transfer_id}");
                                            Console.WriteLine($"From: {fromUser}");
                                            Console.WriteLine($"To: {toUser}");
                                            Console.WriteLine($"Type: {transferType.transfer_type_desc}");
                                            Console.WriteLine($"Status: {transferStatus.transfer_status_desc.ToUpper()}");
                                            Console.WriteLine($"Amount: {transfer.amount:c2}");
                                            WriteLineBreak();

                                            apiService.UpdateTransfer(transfer);

                                            menuSelection = -1;
                                        }
                                        else if (menuSelection == 2)
                                        {
                                            transfer.transfer_status_id = 3;
                                            transferStatus = apiService.GetTransferStatusById(transfer.transfer_status_id);

                                            WriteLineBreak();
                                            Console.WriteLine("Transfer Details");
                                            WriteLineBreak();
                                            Console.WriteLine($"Id: {transfer.transfer_id}");
                                            Console.WriteLine($"From: {fromUser}");
                                            Console.WriteLine($"To: {toUser}");
                                            Console.WriteLine($"Type: {transferType.transfer_type_desc}");
                                            Console.WriteLine($"Status: {transferStatus.transfer_status_desc.ToUpper()}");
                                            Console.WriteLine($"Amount: {transfer.amount:c2}");
                                            WriteLineBreak();

                                            apiService.UpdateTransfer(transfer);

                                            menuSelection = -1;
                                        }
                                        else
                                        {
                                            menuSelection = -1;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    menuSelection = -1;
                                    Console.WriteLine("Transfer not found");
                                }
                            }
                            if (temp.Count <= 0)
                            {
                                Console.WriteLine("No pending requests found");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No pending requests found");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No transfers found");
                    }

                    menuSelection = -1;
                }
                else if (menuSelection == 4)
                {

                    List<API_User> allUsers = apiService.GetListOfUsers();
                    int userSelected = -1; //to store user input for id
                    decimal amountToTransfer = -1M; // to store user input for amount

                    WriteLineBreak();
                    Console.WriteLine("Users");
                    Console.WriteLine($"ID {"Name".PadLeft(16)}");
                    WriteLineBreak();

                    List<int> ids = new List<int>();

                    foreach (API_User user in allUsers)
                    {
                        //do not display current user
                        if (user.UserId != UserService.GetUserId())
                        {
                            ids.Add(user.UserId);
                            Console.WriteLine($"{user.UserId}   {user.Username.PadLeft(16)}");
                        }

                    }
                    WriteLineBreak();

                    //Get input for UserId to transfer to
                    Console.Write("Enter ID of user you are sending to (0 to cancel) : ");
                    bool isValid = int.TryParse(Console.ReadLine(), out userSelected);
                    isValid = isValid && ids.Contains(userSelected);
                    if (!isValid)
                    {
                        userSelected = 0;
                        Console.WriteLine("Invalid input. Please enter only a user id number.");
                    }
                    if (userSelected == 0)
                    {
                        menuSelection = -1;
                        continue;
                    }

                    //Get input for amount of TE Bucks to transfer
                    if (isValid)
                    {
                        Console.Write("Enter amount: ");
                        isValid = decimal.TryParse(Console.ReadLine(), out amountToTransfer);
                        decimal databaseConstraint = 99999999999.99M;
                        if (!isValid || amountToTransfer <= 0 || amountToTransfer > databaseConstraint)
                        {
                            Console.WriteLine("Invalid input. Please enter a valid amount");
                            menuSelection = -1;
                            continue;
                        }
                    }

                    

                    API_Transfer newTransfer = new API_Transfer(2, 2, UserService.GetUserId(), userSelected, amountToTransfer);

                    API_Transfer completedTransfer = apiService.CreateTransfer(newTransfer);


                    if(completedTransfer.transfer_status_id == 3)
                    {
                        Console.WriteLine("Transfer was rejected due to insufficient funds");
                    }
                    else
                    {
                        Console.WriteLine("Completed transfer");
                    }
                    

                    menuSelection = -1;

                }
                else if (menuSelection == 5)
                {
                    List<API_User> allUsers = apiService.GetListOfUsers();
                    int userSelected = -1; //to store user input for id
                    decimal amountToRequest = -1M; // to store user input for amount

                    WriteLineBreak();
                    Console.WriteLine("Users");
                    Console.WriteLine($"ID {"Name".PadLeft(16)}");
                    WriteLineBreak();

                    List<int> ids = new List<int>();

                    foreach (API_User user in allUsers)
                    {
                        //do not display current user
                        if (user.UserId != UserService.GetUserId())
                        {
                            ids.Add(user.UserId);
                            Console.WriteLine($"{user.UserId}   {user.Username.PadLeft(16)}");
                        }

                    }
                    WriteLineBreak();

                    //Get input for UserId to transfer to
                    Console.Write("Enter ID of user you are requesting from (0 to cancel) : ");
                    bool isValid = int.TryParse(Console.ReadLine(), out userSelected);
                    isValid = isValid && ids.Contains(userSelected);
                    if (!isValid)
                    {
                        userSelected = 0;
                        Console.WriteLine("Invalid input. Please enter only a user id number.");
                    }
                    if (userSelected == 0)
                    {
                        menuSelection = -1;
                        continue;
                    }

                    //Get input for amount of TE Bucks to transfer
                    if (isValid) {
                        Console.Write("Enter amount: ");
                        decimal databaseConstraint = 99999999999.99M;
                        isValid = decimal.TryParse(Console.ReadLine(), out amountToRequest);
   
                        if (!isValid || amountToRequest <= 0 || amountToRequest > databaseConstraint)
                        {
                            Console.WriteLine("Invalid input. Please enter a valid amount.");
                            menuSelection = -1;
                            continue;
                        }
                    }


                    API_Transfer newTransfer = new API_Transfer(1, 1, userSelected, UserService.GetUserId(), amountToRequest);

                    API_Transfer completedTransfer = apiService.CreateTransfer(newTransfer);

                    Console.WriteLine("Request Sent");

                    menuSelection = -1;

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }         
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }

        private static void WriteLineBreak()
        {
            Console.WriteLine("------------------------");
        }
    }
}

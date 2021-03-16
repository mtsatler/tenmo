using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;
using System.Net.Http;

namespace TenmoClient
{
    public class APIService
    {
        private readonly string API_URI = "";
        private readonly RestClient client = new RestClient();
        
        public APIService(string api_uri)
        {
            API_URI = api_uri;
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
        }

        public List<API_Account> GetAccountsOfCurrentUser()
        {
           
            RestRequest request = new RestRequest(API_URI + "accounts/user/" + UserService.GetUserId().ToString());
            IRestResponse<List<API_Account>> response = client.Get<List<API_Account>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<API_Transfer> GetTransfersOfCurrentUser()
        {
            RestRequest request = new RestRequest(API_URI + "transfer/users/" + UserService.GetUserId().ToString());
            IRestResponse<List<API_Transfer>> response = client.Get<List<API_Transfer>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;

        }

        public API_Transfer GetTransferById(int transferId)
        {
            RestRequest request = new RestRequest(API_URI + "transfer/" + transferId);
            IRestResponse<API_Transfer> response = client.Get<API_Transfer>(request);

            if(response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;

        }

        public API_TransferStatus GetTransferStatusById(int transferId)
        {
            RestRequest request = new RestRequest(API_URI + "transfer/status/" + transferId);
            IRestResponse<API_TransferStatus> response = client.Get<API_TransferStatus>(request);

            if(response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public API_TransferType GetTransferTypeById(int transferId)
        {
            RestRequest request = new RestRequest(API_URI + "transfer/type/" + transferId);
            IRestResponse<API_TransferType> response = client.Get<API_TransferType>(request);

            if(response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public API_Transfer CreateTransfer(API_Transfer newTransfer)
        {
            RestRequest request = new RestRequest(API_URI + "transfer");
            request.AddJsonBody(newTransfer);
            IRestResponse<API_Transfer> response = client.Post<API_Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<API_User> GetListOfUsers()
        {
            RestRequest request = new RestRequest(API_URI + "users");
            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);

            if(response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public string GetUserNameFromId(int userId)
        {
            
            RestRequest request = new RestRequest(API_URI + "users/id/" + userId);
            IRestResponse<API_User> response = client.Get<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data.Username;
            }
            return null;

        }

        private void ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new HttpRequestException("Error occurred - unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new HttpRequestException("Authorization is required for this endpoint.Please log in.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new HttpRequestException("You do not have permission to perform the requested action");
                }
                else
                {
                    throw new HttpRequestException("Error occurred - received non-success response: " + (int)response.StatusCode);
                }
            }
        }

        public API_Transfer UpdateTransfer(API_Transfer newTransfer)
        {
            RestRequest request = new RestRequest(API_URI + "transfer/" + newTransfer.transfer_id);
            request.AddJsonBody(newTransfer);
            IRestResponse<API_Transfer> response = client.Put<API_Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }



    }
}

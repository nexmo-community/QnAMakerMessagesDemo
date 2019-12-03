# QnAMakerMessagesDemo
## PreRequs
This Assumes that you already have a QnA Maker bot setup and published, as well as a nexmo account and Messages application
* Visual Studio 2019 version 16.3 or higher
* A Nexmo account [Sign up here](https://dashboard.nexmo.com/sign-up)
* An Azure Account
* Optional: [Ngrok](https://ngrok.com/) for test deployment
* Optional: For Facebook Messenger we will need to link a Facebook Page to our Nexmo account - see how [here](https://developer.nexmo.com/use-cases/sending-facebook-messenger-messages-with-messages-api). Completing part 2 of this will create a Nexmo App with a linked Facebook Page, make sure to save the private key file that is generated for this application.

## Setup
Open in Visual Studio,

Replace values in appsettings.json with appropriate values from your nexmo app, replace constants in, Questioner.cs with values from QnAMaker app
Expose to the internet, configure Nexmo app appropriately

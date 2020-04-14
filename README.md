# NopCommerce_FirstData
A FirstData payment plugin for NopCommerce

## Demo Account
To test the plugin in a sandbox, you'll need a demo account.  Create one at https://provisioning.demo.globalgatewaye4.firstdata.com/signup.

## Environment
Each version of NopCommerce requires a different version of the plugin.  There is a branch of the plugin for each matching branch of NopCommerce, `NopCommerce_4.10`

To run multiple versions locally, clone [NopCommerce releases](https://github.com/nopSolutions/nopCommerce/releases) to individual directories and then clone the matching branch of this plugin into the \Plugins directory.

Open the NopCommerce solution and right click the Plugins folder, Add -> Existing Project -> find Plugins\NopCommerce_FirstData\BitShift.Plugin.Payments.FirstData.csproj.  Do the same for BitShift.Plugin.Payments.FirstData.Tests.csproj in the Tests folder.

Building the solution should now output the plugin files to Presentation\Nop.Web\Plugins\BitShift.Payments.FirstData.  The plugin will be available to install in the Admin section.

Installing the plugin will add the BitShift_FirstData_* tables to the database.  Uninstalling the plugin through the UI will remove the tables.

## Configuration
First Data requires 4 distinct pieces of info to accept transactions from your store: Gateway ID, Password, Key ID, and HMAC.  If you are testing out the plugin, the first step is to register for a demo account with First Data here. Once you are in, click Administration at the top right, then Terminals.  Select the Terminal with ECOMM in the name.

* Image *

In the middle of this form, you will see Password.  You might have to click Generate to get one.  This is the password field for the plugin and the Gateway ID is here, too. It's important to note that the Password is separate from the username/password you used to log in.  Click on the API Access tab and generate a new HMAC Key.  The Key ID is on this page, also.  Click Update at the bottom to save the HMAC.

* Image *

You'll have to do it again if you are using the Production version.  The demo info will only work in Sandbox mode and the Production info will only work if Sandbox is turned off.  If you have any other questions don't hesitate to use the contact form.

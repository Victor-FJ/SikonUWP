using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SikonUWP.Common
{
    /// <summary>
    /// A helper class for displaying popup messages to the user
    /// </summary>
    public class MessageDialogUtil
    {
        /// <summary>
        /// Displays a popup message the user can close
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="title">The title of the message</param>
        public static async Task MessageDialogAsync(string title, string message)
        {
            MessageDialog messageDialog = new MessageDialog(message, title);
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Displays a popup yes or no quistion for the user to answer
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="title">The title of the message</param>
        /// <returns>Boolean response where true is a yes</returns>
        public static async Task<bool> InputDialogAsync(string title, string message)
        {
            MessageDialog dialog = new MessageDialog(message, title);
            dialog.Commands.Add(new UICommand("Yes"));
            dialog.Commands.Add(new UICommand("No"));
            dialog.DefaultCommandIndex = 1;
            dialog.CancelCommandIndex = 1;
            IUICommand result = await dialog.ShowAsync();
            return result.Label == "Yes";
        }

        /// <summary>
        /// Displays a popup text request for the user to fill
        /// </summary>
        /// <param name="message">The message text</param>
        /// <param name="title">The title of the message</param>
        /// <returns>String response</returns>
        public static async Task<string> TextInputDialogAsync(string title, string message)
        {
            TextBlock messageBlock = new TextBlock();
            messageBlock.Text = message;
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            inputTextBox.Width = 400;
            StackPanel contentPanel = new StackPanel();
            contentPanel.Children.Add(messageBlock);
            contentPanel.Children.Add(inputTextBox);

            ContentDialog dialog = new ContentDialog();
            dialog.Content = contentPanel;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            return null;
        }
    }
}

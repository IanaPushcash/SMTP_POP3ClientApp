using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using OpenPop.Pop3;

namespace PRlab4WPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string AdFilePath;
		private Pop3Client _pop3Client;
		public Attachment AtmentForDwnld;
		public MainWindow()
		{
			InitializeComponent();
			_pop3Client = new Pop3Client();
		}

		private void Mails_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl && ((TabControl)e.Source).SelectedIndex == 1)
			{
				try
				{
					if (_pop3Client.Connected)
						_pop3Client.Disconnect();
					_pop3Client.Connect("127.0.0.1", 110, false);
					_pop3Client.Authenticate("puscas@lab4.pr", "aaaa");
					int count = _pop3Client.GetMessageCount();
					this.lvMails.Items.Clear();
					for (int i = count; i >= 1; i -= 1)
					{
						var message = _pop3Client.GetMessage(i).ToMailMessage();
						this.lvMails.Items.Add(new Message()
						{
							Id = count - i + 1,
							From = message.From.ToString(),
							To = message.To.First().ToString(),
							Subject = message.Subject,
							Attachment = message.Attachments.Count > 0 ? message.Attachments.First().Name : "",
							Body = message.Body,
							AttachmentObj = message.Attachments.Count > 0 ? message.Attachments.First() : null
						});
					}

				}
				catch (Exception ex)
				{
					MessageBox.Show(this, "Error occurred retrieving mail. " + ex.Message, "POP3 Retrieval");
				}
			}
		}

		private void btnAddAt_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == true)
			{
				lbAtName.Content = ofd.FileName;
				AdFilePath = ofd.FileName;
			}
		}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				MailAddress from = new MailAddress("puscas@lab4.pr", "Iana");]
				MailAddress to = new MailAddress(tbTo.Text);
				MailMessage m = new MailMessage(from, to)
				{
					Subject = tbTheme.Text,
					Body = tbMailContent.Text,
					IsBodyHtml = tbMailContent.Text.StartsWith("<")
				};
				if (!string.IsNullOrEmpty(AdFilePath)) m.Attachments.Add(new Attachment(AdFilePath));
				SmtpClient smtp = new SmtpClient("127.0.0.1", 25)
				{
					EnableSsl = false
				};
				smtp.Send(m);
				tbTheme.Text = "";
				tbMailContent.Text = "";
				lbAtName.Content = "Is sended";
				AdFilePath = "";
			}
			catch (Exception exception)
			{
				lbAtName.Content = exception.Message;
			}
		}



		private void lvMails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var item = ((FrameworkElement)e.OriginalSource).DataContext as Message;
			if (item == null) return;
			this.lbFrom.Content = item.From;
			this.lbSubject.Content = item.Subject;
			this.wbBody.NavigateToString(Regex.Replace(item.Body, "\\r\\n[^ <]", "<br>"));
			AtmentForDwnld = item.AttachmentObj;
		}


		private void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			if (AtmentForDwnld == null) return;
			SaveFileDialog sfd = new SaveFileDialog
			{
				Title = "Save attachment",
				FileName = AtmentForDwnld.Name,
				Filter = "All files (*.*)|*.*"
			};
			if (sfd.ShowDialog() == true)
			{
				byte[] buffer = new byte[AtmentForDwnld.ContentStream.Length];
				AtmentForDwnld.ContentStream.Read(buffer, 0, buffer.Length);
				FileStream file = new FileStream(sfd.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
				file.Write(buffer, 0, buffer.Length);
				file.Dispose();
			}
		}
	}
}

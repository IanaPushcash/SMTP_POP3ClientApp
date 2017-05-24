using System.Net.Mail;

namespace PRlab4WPF
{
	class Message
	{
		public int Id { get; set; }
		public string From { get; set; }
		public string To { get; set; }
		public string Subject { get; set; }
		public string Attachment { get; set; }
		public string Body { get; set; }
		public Attachment AttachmentObj { get; set; }
	}
}

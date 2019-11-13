// Mailkit utility function for reading emails
// David Piao
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

public class MailRepository
{
    public ImapClient client = new ImapClient();
    private readonly string mailServer, login, password;
    private readonly int port;
    private readonly bool ssl;

    public MailRepository(string mailServer, int port, bool ssl, string login, string password)
    {
        this.mailServer = mailServer;
        this.port = port;
        this.ssl = ssl;
        this.login = login;
        this.password = password;
    }

    public bool connect()
    {
        try
        {
            client.Connect(mailServer, port, ssl);
            // Note: since we don't have an OAuth2 token, disable
            // the XOAUTH2 authentication mechanism.
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(login, password);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        return false;
    }

    public bool disconnect()
    {
        try
        {
            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        return false;
    }
    public IEnumerable<string> get_from(string from)
    {
        try
        {
            var messages = new List<string>();
            
            // The Inbox folder is always available on all IMAP servers...
            var all_email = client.GetFolder(SpecialFolder.All);
            all_email.Open(FolderAccess.ReadWrite);
            var results = all_email.Search(SearchQuery.FromContains(from));
            foreach (var uniqueId in results)
            {
                var message = all_email.GetMessage(uniqueId);
                if (message.HtmlBody != null)
                    messages.Add(message.HtmlBody);
                else
                    messages.Add(message.TextBody);
                //Mark message as read
                all_email.AddFlags(uniqueId, MessageFlags.Seen, true);
            }
            if(messages.Count == 0)
            {
                // Check Spam
                var spam = client.GetFolder(SpecialFolder.Junk);
                spam.Open(FolderAccess.ReadWrite);
                results = spam.Search(SearchQuery.FromContains(from));
                foreach (var uniqueId in results)
                {
                    var message = spam.GetMessage(uniqueId);
                    if (message.HtmlBody != null)
                        messages.Add(message.HtmlBody);
                    else
                        messages.Add(message.TextBody);
                    //Mark message as read
                    spam.AddFlags(uniqueId, MessageFlags.Seen, true);
                }
            }
            return messages;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        return null;
    }

    public IEnumerable<string> get_unread_emails()
    {
        try
        {
            var messages = new List<string>();
            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
            foreach (var uniqueId in results.UniqueIds)
            {
                var message = inbox.GetMessage(uniqueId);

                messages.Add(message.HtmlBody);

                //Mark message as read
                //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
            }
            return messages;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        return null;
    }

    public bool mark_all_as_read()
    {
        try
        {
            // The Inbox folder is always available on all IMAP servers...
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadWrite);
            var results = inbox.Search(SearchOptions.All, SearchQuery.NotSeen);
            foreach (var uniqueId in results.UniqueIds)
            {
                inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
        }
        return false;
    }
}
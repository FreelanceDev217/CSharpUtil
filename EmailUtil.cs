// Function to be used for getting confirmation email
// David Piao
async Task<string> get_confirm_email(string from)
{
    string email = "";
    try
    {
        MainApp.log_error($"#{m_param.id}: Connecting to email...");
        MailRepository repo = new MailRepository("imap.gmail.com", 993, true, m_param.email, m_param.password);
        repo.connect();

        IEnumerable<string> unread;

        var w = new Stopwatch();
        w.Start();
        while (MainApp.g_stopped == false)
        {
            if (w.ElapsedMilliseconds > 120 * 1000)
            {
                MainApp.log_error($"#{m_param.id}: Confirmation email is not received in time limit.");
                return "";
            }
            unread = repo.get_from(from);
            if (unread == null)
            {
                MainApp.log_error("Email client authentication failed.");
                return null;
            }
            if (unread.Count() == 0)
                continue;

            // extract necessary part
            email = unread.Last(); // last will be the latest
            MainApp.log_error($"#{m_param.id}: New email from {m_param.url} retrieved.");
            break;
        }
        repo.disconnect();
        return email;
    }
    catch(Exception ex)
    {
        MainApp.log_error($"#{m_param.id}: Exception occured at step: login. {ex.Message}\n{ex.StackTrace}");
    }
    return "";
}
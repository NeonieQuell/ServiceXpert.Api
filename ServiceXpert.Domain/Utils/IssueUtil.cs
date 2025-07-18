﻿namespace ServiceXpert.Domain.Utils;
public static class IssueUtil
{
    public static int GetIdFromIssueKey(string issueKey)
    {
        try
        {
            if (int.TryParse(issueKey.Split('-')[1], out int issueId))
            {
                return issueId;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            throw new IndexOutOfRangeException("Failed to extract Id from Key", e);
        }

        return -1;
    }
}

// first modified in 18/04/20
//
using System;

namespace Library_app_test.Data
{
    internal record Author(int Id, string Name);
    internal record Book(int Id, string Title, int? AuthorId, string? ISBN, int? PublishedYear, int Copies);
    internal record Member(int Id, string FullName, string? Email, string? Phone);
    internal record Issue(int Id, int BookId, int MemberId, DateTime IssueDate, DateTime DueDate, DateTime? ReturnDate);

    // View model for displaying issue records with joined book/member info
    internal record IssueView(int Id, string BookTitle, string MemberName, DateTime IssueDate, DateTime DueDate, DateTime? ReturnDate, string Status);
}

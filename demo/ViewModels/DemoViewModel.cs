using demo.Models;
using LisBot.Common.Telegram.Attributes;

namespace demo.ViewModels;

public class DemoViewModel
{
    [ValidateStateProperty("User Full Name")]
    public string UserFullName
    {
        get
        {
            return _userFullName ?? throw new ArgumentNullException(nameof(_userFullName));
        }
        set
        {
            _userFullName = value;
        }
    }

    private string? _userFullName;

    [ValidateStateProperty("Library Rating")]
    public DemoEnum LibraryRating
    {
        get
        {
            return _libraryRating ?? throw new ArgumentNullException(nameof(_libraryRating));
        }
        set
        {
            _libraryRating = value;
        }
    }

    private DemoEnum? _libraryRating;

    [ValidateStateProperty("List Object")]
    public DemoListObject ListObject
    {
        get
        {
            return _listObject ?? throw new ArgumentNullException(nameof(_listObject));
        }
        set
        {
            _listObject = value;
        }
    }

    private DemoListObject? _listObject;
}

public enum DemoEnum
{
    None,
    Awesome,
    Mid,
    DeeplyUpsetting
}
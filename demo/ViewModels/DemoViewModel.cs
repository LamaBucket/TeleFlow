using demo.Models;
using TeleFlow.Attributes;

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


    [ValidateStateProperty("Date")]
    public DateOnly SelectedDate
    {
        get
        {
            return _selectedDate ?? throw new ArgumentNullException(nameof(_selectedDate));
        }
        set
        {
            _selectedDate = value;
        }
    }

    private DateOnly? _selectedDate;
    
    
    [ValidateStateProperty("Phone Number")]
    public string PhoneNumber
    {
        get
        {
            return _phoneNumber ?? throw new ArgumentNullException(nameof(_phoneNumber));
        }
        set
        {
            _phoneNumber = value;
        }
    }

    private string? _phoneNumber;
}

public enum DemoEnum
{
    None,
    Awesome,
    Mid,
    DeeplyUpsetting
}
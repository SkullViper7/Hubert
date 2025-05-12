using UnityEngine;

[CreateAssetMenu(fileName = "UISprite", menuName = "UISprite/Create new UI sprite")]
public class InputUIData : ScriptableObject
{
    /// <summary>
    /// The sprite of the input when player is on keyboard and mouse.
    /// </summary>
    [SerializeField]
    private Sprite _keyboardMouseSprite;

    /// <summary>
    /// Gets the sprite of the input when player is on keyboard and mouse.
    /// </summary>
    public Sprite KeyboardMouseSprite { get { return _keyboardMouseSprite; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a generic gamepad.
    /// </summary>
    [Space, SerializeField]
    private Sprite _genericGamepadSprite;

    /// <summary>
    /// Gets the sprite of the input when player is on a generic gamepad.
    /// </summary>
    public Sprite GenericGamepadSprite { get { return _genericGamepadSprite; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a Xbox controller.
    /// </summary>
    [Space, SerializeField]
    private Sprite _xboxSprite;

    /// <summary>
    /// Gets the sprite of the input when player is on a Xbox controller.
    /// </summary>
    public Sprite XboxSprite { get { return _xboxSprite; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a Dualshock3 controller.
    /// </summary>
    [Space, SerializeField]
    private Sprite _dualshock3Sprite;

    /// <summary>
    /// Gets the sprite of the input when player is on a Dualshock3 controller.
    /// </summary>
    public Sprite Dualshock3Sprite { get { return _dualshock3Sprite; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a Dualshock4 controller.
    /// </summary>
    [Space, SerializeField]
    private Sprite _dualshock4Sprite;

    /// <summary>
    /// Gets the sprite of the input when player is on a Dualshock4 controller.
    /// </summary>
    public Sprite Dualshock4Sprite { get { return _dualshock4Sprite; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a Dualsense controller.
    /// </summary>
    [Space, SerializeField]
    private Sprite _dualsense;

    /// <summary>
    /// Gets the sprite of the input when player is on a Dualsense controller.
    /// </summary>
    public Sprite Dualsense { get { return _dualsense; } private set { } }

    /// <summary>
    /// The sprite of the input when player is on a switch controller.
    /// </summary>
    [Space, SerializeField]
    private Sprite _switchSprite;

    /// <summary>
    /// Gets the sprite of the input when player is on a switch controller.
    /// </summary>
    public Sprite SwitchSprite { get { return _switchSprite; } private set { } }

    /// <summary>
    /// Name of the input
    /// </summary>
    private string _inputName;

    /// <summary>
    /// Gets the name of the input.
    /// </summary>
    public string InputName { get { return _inputName; } private set { _inputName = this.name; } }
}

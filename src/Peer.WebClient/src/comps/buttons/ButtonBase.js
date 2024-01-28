import React from 'react'

const ButtonBase = (props) => {

    const {
        onClick = () => {},
        disabled,
        text,
        defaultStyle = "",
        hoverStyle,
        icon
    } = props;

    const baseStyle = "py-2 px-4 rounded-full font-semibold w-fit flex flex-row items-center shrink-0 cursor-pointer"
    const enabledStyle = `${baseStyle} ${defaultStyle} ${hoverStyle ? `hover:${hoverStyle}` : ""}`  // works if hoverStyle is just one string, e.g. bg-red-500
    const disabledStyle = `${baseStyle} bg-gray-100 border border-gray-300 text-gray-300 cursor-not-allowed`

    return (
        <button
            disabled={disabled}
            onClick={onClick}
            className={`${disabled ? disabledStyle : enabledStyle}`}>
            {
                icon &&
                <div className='mr-2'>{icon}</div>
            }
            <p className="shrink-0 truncate">{text}</p> {/* truncate makes it... not truncate? */}
        </button>
    )
}

export default ButtonBase
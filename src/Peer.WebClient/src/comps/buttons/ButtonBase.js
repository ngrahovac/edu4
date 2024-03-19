import React from 'react'

const ButtonBase = (props) => {

    const {
        onClick = () => {},
        disabled = false,
        text,
        defaultStyle = "",
        hoverStyle = undefined,
        icon
    } = props;

    const baseStyle = "rounded-full py-2 px-4 font-semibold w-fit flex flex-row items-center shrink-0 cursor-pointer"
    const enabledStyle = `${baseStyle} ${defaultStyle} ${hoverStyle ? hoverStyle : ''}`  // works if hoverStyle is just one string, e.g. bg-red-500
    const disabledStyle = `${baseStyle} text-gray-300 cursor-not-allowed`

    return (
        <div
            disabled={disabled}
            onClick={onClick}
            className={`${disabled ? disabledStyle : enabledStyle}`}>
            {
                icon &&
                <div className='mr-2'>{icon}</div>
            }
            <p className="shrink-0 truncate">{text}</p> {/* truncate makes it... not truncate? */}
        </div>
    )
}

export default ButtonBase
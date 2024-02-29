import React from 'react'

export const HatSearchParam = (props) => {
    const {
        selected = false,
        onSelected = () => { },
        children
    } = props;

    const backgroundColor = !selected ? "bg-gray-100" : "bg-indigo-400";
    const textColor = !selected ? "text-gray-600" : "text-gray-50";

    return (
        <div
            onClick={onSelected}
            className={`${backgroundColor} ${textColor} font-semibold rounded-full px-3 py-1 cursor-pointer`}>
            {children}
        </div>
    )
}

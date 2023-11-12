import React from 'react'

const TabulatedMenuItem = (props) => {
    const {
        item,
        selected = false,
        onItemSelected = () => { }
    } = props;

    return (
        <div
            onClick={() => onItemSelected(item)}
            className={`flex shrink-0 cursor-pointer text-base ${selected ? 'text-indigo-600 font-semibold border-b-2 border-indigo-600' : 'text-gray-800 font-medium'}`}>
                {item}
        </div>
    )
}

export default TabulatedMenuItem
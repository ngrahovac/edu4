import React, { useState } from 'react'

const TableRow = (props) => {
    const {
        children,
        selected = false,
    } = props;

    return (
        <tr className={`cursor-pointer border-b border-gray-100 ${selected ? 'hover:bg-indigo-400' : 'hover:bg-indigo-50'} ${selected ? 'bg-indigo-300' : ''}`}>
            {children}
        </tr>
    )
}

export default TableRow
import React from 'react'

const Table = (props) => {
    const {
        columns,
        selectedCount,
        widths = {},
        children
    } = props;

    let index = 1;
    let numberedColumns = columns.map(c => ({ id: index++, column: c })); // generating keys during rendering?

    return (
        <table className='text-left table-auto w-full overflow-x-scroll'>
            <thead>
                <tr className='border-b border-gray-400'>
                    {
                        numberedColumns.map(c => <th key={c.id} className={`${widths[c.column] ?? ''} p-4`}>
                            {c.column}
                        </th>)
                    }
                </tr>
            </thead>
            <tbody>
                {children}
            </tbody>
            <tfoot className=' border-gray-300'>
                <tr>
                    <td className='text-left p-4 h-12 uppercase tracking-wide'>
                        <p>{`Selected: ${selectedCount}`}</p>
                    </td>
                </tr>
            </tfoot>
        </table>
    )
}

export default Table
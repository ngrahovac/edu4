import React from 'react'

const ApplicationsTable = ({ children }) => {
    return (
        <table className='w-full table-fixed text-left'>
            {children}
        </table>
    )
}

ApplicationsTable.Header = ({ children }) => <thead><tr className='border-b-2 border-gray-200'>{children}</tr></thead>
ApplicationsTable.Header.Cell = ({ children }) => <th className='font-medium truncate text-sm p-4 text-gray-400 uppercase text-left'>{children}</th>

ApplicationsTable.Body = ({ children }) => <tbody>{children}</tbody>
ApplicationsTable.Body.Row = ({ children, selected = false }) => <tr className={`cursor-pointer ${selected ? 'hover:bg-indigo-400 bg-indigo-300' : 'hover:bg-gray-200 bg:gray-100'}`}>{children}</tr>
ApplicationsTable.Body.Cell = ({ children }) => <td className={`text-gray-800 p-4 text-left truncate`}>{children}</td>

ApplicationsTable.Footer = ({ selectedCount = 0 }) => <tfoot className='border-t-2 border-gray-200'>
    <tr>
        <td className='text-left p-4 h-12 uppercase tracking-wider text-slate-400 text-sm font-medium'>
            <p>{`Selected: ${selectedCount}`}</p>
        </td>
    </tr>
</tfoot>

export default ApplicationsTable
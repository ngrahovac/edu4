import React from 'react'

const ApplicationsTable = ({ children }) => {
    return (
        <div className='h-96 relative'>
            <table className='w-full table-fixed text-left'>
                {children}
            </table>
        </div>
    )
}

ApplicationsTable.Header = ({ children }) => <thead><tr className='border-b-2 border-gray-200'>{children}</tr></thead>
ApplicationsTable.Header.Cell = ({ children }) => <th className='font-medium truncate text-sm px-4 py-2 text-gray-400 uppercase text-left'>{children}</th>

ApplicationsTable.Body = ({ children }) => <tbody>{children}</tbody>
ApplicationsTable.Body.Row = ({ children, selected = false }) => <tr className={`${selected ? 'hover:bg-indigo-400 bg-indigo-300' : 'hover:bg-gray-200 bg:gray-100'}`}>{children}</tr>
ApplicationsTable.Body.Cell = ({ children }) => <td className={`text-gray-800 p-4 text-left truncate`}>{children}</td>

ApplicationsTable.Footer = ({ children }) => <tfoot className='border-t-2 border-gray-200'>
    {children}
</tfoot>

ApplicationsTable.Footer.Row = ({ children }) => <tr className='absolute bottom-4 w-full flex justify-between align-middle items-center place-items-center h-8'>{children}</tr>

ApplicationsTable.Footer.Cell = ({ children, collspan = 1 }) => <td colSpan={collspan} className='text-left px-4 py-1 h-8 uppercase tracking-wider text-slate-400 text-sm font-medium flex content-center'>
    {children}
</td>

export default ApplicationsTable
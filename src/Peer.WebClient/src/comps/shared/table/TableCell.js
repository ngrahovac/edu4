import React from 'react'

const TableCell = (props) => {
  const {
    children,
  } = props;

  return (
    <td className='p-4 truncate'>
      {children}
    </td>

  )
}

export default TableCell
import React from 'react'

const ClearSearchFilter = (props) => {
  const {
    onClick
  } = props;

  return (
    <div
      onClick={onClick}
      className='flex flex-row shrink-0 items-center py-2 w-20 hover:text-red-500 mt-2'>
      <svg
        className="w-4 h-4 mr-1 cursor-pointer"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        stroke-width="2"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
      </svg>
      <p className='text-sm'>Clear</p>
    </div>
  )
}

export default ClearSearchFilter
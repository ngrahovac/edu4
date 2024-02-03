import React from 'react'

const ClearSearchFilter = (props) => {
  const {
    onClick
  } = props;

  return (
    <div
      onClick={onClick}
      className='cursor-pointer hover:text-indigo-600 flex shrink-0 items-center gap-x-1 font-semibold text-sm uppercase tracking-wide text-gray-400'>

      <p className='text-sm'>Clear</p>
      <svg
        className="w-4 h-4"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        strokeWidth="3"
        stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </div>
  )
}

export default ClearSearchFilter
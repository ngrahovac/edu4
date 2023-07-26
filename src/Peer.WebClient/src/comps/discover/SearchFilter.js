import React from 'react'

const SelectedDiscoveryParameter = (props) => {
  const {
    value,
    onRemoved
  } = props;

  return (
    <div className='flex flex-row shrink-0 items-center place-content-center rounded-full bg-indigo-200 px-4 py-1'>
      <svg
        onClick={onRemoved}
        className="w-4 h-4 mr-1 cursor-pointer"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        stroke-width="2"
        stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
      </svg>
      <p className='text-sm'>{value}</p>
    </div>
  )
}

export default SelectedDiscoveryParameter

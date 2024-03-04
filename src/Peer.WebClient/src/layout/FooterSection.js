import React from 'react'

const FooterSection = ({children}) => {
  return (
    <p className='font-semibold text-lg text-gray-200 mb-2'>
        {children}
    </p>
  )
}

export default FooterSection
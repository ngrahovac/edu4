import React from 'react'

const FooterItem = ({children}) => {
  return (
    <p className='text-base text-gray-200 font-regular cursor-pointer hover:text-indigo-200'>
        {children}
    </p>
  )
}

export default FooterItem
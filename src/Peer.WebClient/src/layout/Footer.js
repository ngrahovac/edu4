import React from 'react'
import LogoIcon from '../comps/landing/GiantLogo'
import GiantLogo from '../comps/landing/GiantLogo'

const Footer = () => {
  return (
    <div className='flex px-32 justify-between h-64'>
        <GiantLogo></GiantLogo>
        <p>Platform</p>
        <p>Community</p>
        <p>Stay in the loop</p>
    </div>
  )
}

export default Footer
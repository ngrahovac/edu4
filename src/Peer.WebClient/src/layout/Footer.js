import React from 'react'
import LogoIcon from '../comps/landing/GiantLogo'
import GiantLogo from '../comps/landing/GiantLogo'
import FooterSection from './FooterSection'
import FooterItem from './FooterItem'
import FooterCTA from './FooterCTA'

const Footer = () => {
    return (
        <div className='flex px-32 justify-between h-80 bg-gray-600 py-16'>
            <div className='flex gap-x-32 items-start'>
                <GiantLogo></GiantLogo>

                <div className='flex flex-col gap-y-2'>
                    <FooterSection>Platform</FooterSection>
                    <FooterItem>About</FooterItem>
                    <FooterItem>Contact</FooterItem>
                    <FooterItem>Terms and conditions</FooterItem>
                    <FooterItem>Privacy policy</FooterItem>
                </div>

                <div className='flex flex-col gap-y-2'>
                    <FooterSection>Community</FooterSection>
                    <FooterItem>Tell us what you think</FooterItem>
                    <FooterItem>Report a bug</FooterItem>
                    <FooterItem>Become a sponsor</FooterItem>
                    <FooterItem>Ways to contribute</FooterItem>
                </div>
            </div>
            <div className='flex flex-col text-gray-200'>
                <FooterCTA></FooterCTA>
            </div>
        </div>
    )
}

export default Footer
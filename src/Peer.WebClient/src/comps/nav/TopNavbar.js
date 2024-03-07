import React, { useState } from 'react'
import { Outlet } from 'react-router-dom'
import Avatar from './Avatar'
import LogoIcon from './LogoIcon'
import BorderlessButton from '../buttons/BorderlessButton'
import { Link } from 'react-router-dom'
import AccentButton from '../buttons/AccentButton'
import TopNavbarContextMenu from './TopNavbarContextMenu'
import { useAuth0 } from '@auth0/auth0-react'
import GlobalSearch from './GlobalSearch'
import TopNavbarItem from './TopNavbarItem'

const TopNavbar = () => {
    const [topNavbarContextMenuHidden, setTopNavbarContextMenuHidden] = useState(true);

    const { user, isAuthenticated } = useAuth0();

    function toggleTopNavbarContextMenuVisibility() {
        setTopNavbarContextMenuHidden(!topNavbarContextMenuHidden);
    }

    const topNavbarContextMenu = (
        <div className='w-64 fixed top-14 right-8 z-50'>
            <TopNavbarContextMenu
                hidden={topNavbarContextMenuHidden}>
            </TopNavbarContextMenu>
        </div>);

    return (isAuthenticated &&
        <>
            <div className='fixed w-full h-16 bg-white flex justify-between px-8 z-40'>
                {/* navbar items on the left */}
                <div className='flex items-center gap-x-4'>
                    <LogoIcon></LogoIcon>

                    <GlobalSearch></GlobalSearch>
                </div>

                {/* navbar items on the right */}
                <div className='flex items-center gap-x-4'>
                    <Link to="/publish">
                        <BorderlessButton
                            text="Publish"
                            icon={<>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
                                </svg>
                            </>}>
                        </BorderlessButton>
                    </Link>

                    <Link to="/applications">
                        <TopNavbarItem
                            text="Applications"
                            icon={<>
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M18 7.5v3m0 0v3m0-3h3m-3 0h-3m-2.25-4.125a3.375 3.375 0 1 1-6.75 0 3.375 3.375 0 0 1 6.75 0ZM3 19.235v-.11a6.375 6.375 0 0 1 12.75 0v.109A12.318 12.318 0 0 1 9.374 21c-2.331 0-4.512-.645-6.374-1.766Z" />
                                </svg>
                            </>}>
                        </TopNavbarItem>
                    </Link>
                </div>

            </div>

            <Outlet></Outlet>
        </>
    )
}

export default TopNavbar
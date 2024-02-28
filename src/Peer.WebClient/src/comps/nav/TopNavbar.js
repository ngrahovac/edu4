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
            {topNavbarContextMenu}

            <div className='fixed w-full h-16 bg-white flex justify-between px-8 z-40'>
                {/* navbar items on the left */}
                <div className='flex items-center gap-x-4'>
                    <LogoIcon></LogoIcon>

                    <GlobalSearch></GlobalSearch>
                </div>

                {/* navbar items on the right */}
                
            </div>

            <Outlet></Outlet>
        </>
    )
}

export default TopNavbar
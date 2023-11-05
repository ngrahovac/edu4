import React from 'react'
import ContextMenu from '../shared/ContextMenu'
import ContextMenuSection from '../shared/ContextMenuSection'
import ContextMenuItem from '../shared/ContextMenuItem'
import { useAuth0 } from '@auth0/auth0-react'

const TopNavbarContextMenu = (props) => {
    const {
        hidden = true
    } = props;

    const { logout } = useAuth0();

    return (
        <ContextMenu
            hidden={hidden}>
            <ContextMenuSection>
                <ContextMenuItem>My projects</ContextMenuItem>
                <ContextMenuItem>My applications</ContextMenuItem>
                <ContextMenuItem>My collaborations</ContextMenuItem>
            </ContextMenuSection>

            <ContextMenuSection>
                <ContextMenuItem>Edit profile</ContextMenuItem>
                <ContextMenuItem>Preferences</ContextMenuItem>
            </ContextMenuSection>

            <ContextMenuSection>
                <ContextMenuItem>About</ContextMenuItem>
                <ContextMenuItem>Reach out</ContextMenuItem>
                <ContextMenuItem>Support the project</ContextMenuItem>
            </ContextMenuSection>

            <ContextMenuSection>
                <ContextMenuItem onClick={() => logout({ returnTo: window.location.origin })}>Log out</ContextMenuItem>
            </ContextMenuSection>
        </ContextMenu>
    )
}

export default TopNavbarContextMenu
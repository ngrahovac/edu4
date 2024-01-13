import React, { useEffect, useState } from 'react'
import ContextMenu from '../ContextMenu';
import ContextMenuSection from '../ContextMenuSection';
import ContextMenuItem from '../ContextMenuItem';
import ContextMenuItemSelected from '../ContextMenuItemSelectionIndicator';

const FilteringContextMenu = (props) => {
    const {
        options,
        onSelectionChange = () => { },
    } = props;

    const [selectedOption, setSelectedOption] = useState(undefined);

    useEffect(() => {
        if (selectedOption !== undefined) {
            onSelectionChange(selectedOption);
        }
    }, [selectedOption])

    return (
        <ContextMenu hidden={false}>
            <ContextMenuSection>
                {
                    options.map(o => <div className='flex flex-row shrink-0 items-center'>
                        <div className={`${selectedOption == o ? '' : 'invisible'}`}>
                            <ContextMenuItemSelected></ContextMenuItemSelected>
                        </div>
                        <ContextMenuItem onClick={() => setSelectedOption(o)}>{o}</ContextMenuItem>
                    </div>)
                }
            </ContextMenuSection>
        </ContextMenu>
    )
}

export default FilteringContextMenu
const ContextMenu = (props) => {

    const {
        children,
        hidden = true
    } = props;

    return (
        <div 
            className={`${hidden ? 'invisible' : ''}`}>
            <div
                className="flex flex-col bg-white rounded-lg rounded-tr-none p-4 divide-y divide-solid shadow-md">
                {children}
            </div>
        </div>
    );
}

export default ContextMenu